using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Husk
{
    [Flags]
    public enum Pipeline { None = 0, F = 1, W = 2, M = 4, E = 8, All = 15 }
    public enum ModuleBuilding { None, TownHall, FishingHarbor, WaterPlant, Recycler, Solar, House }

    public static class BuildingPipelines
    {
        public static Pipeline Inputs(ModuleBuilding building) => building switch
        {
            ModuleBuilding.FishingHarbor => Pipeline.M,
            ModuleBuilding.WaterPlant => Pipeline.E | Pipeline.M,
            ModuleBuilding.Recycler => Pipeline.E,
            ModuleBuilding.House => Pipeline.F | Pipeline.W,
            _ => Pipeline.None // Town Hall I/O deliberately unspecified in Phase 1.
        };
        public static Pipeline Outputs(ModuleBuilding building) => building switch
        {
            ModuleBuilding.FishingHarbor => Pipeline.F,
            ModuleBuilding.WaterPlant => Pipeline.W,
            ModuleBuilding.Recycler => Pipeline.M,
            ModuleBuilding.Solar => Pipeline.E,
            _ => Pipeline.None
        };
        public static Pipeline RequiredModule(ModuleBuilding building) => Inputs(building) | Outputs(building);
        public static Pipeline ForPlacement(ModuleBuilding building, Pipeline existing)
        {
            var result = existing | RequiredModule(building);
            // Stable internal preference: retain earlier F/W/M/E optional lanes. Not gameplay balance.
            for (int bit = (int)Pipeline.E; !IsStandard(result) && bit > 0; bit >>= 1)
                if ((RequiredModule(building) & (Pipeline)bit) == 0) result &= ~(Pipeline)bit;
            return result;
        }
        public static bool IsStandard(Pipeline pipelines)
        {
            int bits = (int)pipelines;
            if ((bits & ~(int)Pipeline.All) != 0) return false;
            int count = 0;
            while (bits != 0) { count += bits & 1; bits >>= 1; }
            return count == 3;
        }
        public static bool Allows(ModuleBuilding building, Pipeline pipelines) =>
            IsStandard(pipelines) && Enum.IsDefined(typeof(ModuleBuilding), building)
            && (pipelines & RequiredModule(building)) == RequiredModule(building);
    }

    public sealed class ModuleCell
    {
        public Vector2Int Position { get; }
        public Pipeline Pipelines { get; internal set; }
        public ModuleBuilding Building { get; internal set; }
        public Pipeline LockedPipelines => BuildingPipelines.RequiredModule(Building);
        // Explicit development supply fixture, independent of concrete storage and building output.
        public Pipeline TestSupply { get; internal set; }
        public bool Supports(Pipeline type) => (Pipelines & type) == type && type != Pipeline.None;
        internal ModuleCell(Vector2Int position, Pipeline pipelines, ModuleBuilding building)
        { Position = position; Pipelines = pipelines; Building = building; }
    }

    public sealed class ModuleLayout
    {
        public const int CoreCost = 1;
        public const int WoodCost = 10;
        public static IReadOnlyList<Vector2Int> Directions { get; } = Array.AsReadOnly(new[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left });
        private readonly Dictionary<Vector2Int, ModuleCell> cells = new();
        private readonly HashSet<Vector2Int> reservedPortCells = new();
        private readonly HashSet<(Vector2Int, Vector2Int)> blockedEdges = new();
        public IReadOnlyDictionary<Vector2Int, ModuleCell> Cells { get; }
        public ResourceState Storage { get; }
        public int Revision { get; private set; }
        public event Action Changed;

        public ModuleLayout(ResourceState storage)
        {
            Storage = storage ?? throw new ArgumentNullException(nameof(storage));
            Cells = new ReadOnlyDictionary<Vector2Int, ModuleCell>(cells);
        }
        public void AddInitial(Vector2Int position, Pipeline pipelines, ModuleBuilding building = ModuleBuilding.None)
        {
            if (!BuildingPipelines.Allows(building, pipelines) || cells.ContainsKey(position) || reservedPortCells.Contains(position))
                throw new ArgumentException("Invalid initial Module.");
            cells.Add(position, new ModuleCell(position, pipelines, building));
            Publish();
        }
        public void ReservePort(Vector2Int owner, Vector2Int seaCell)
        {
            if (!cells.ContainsKey(owner) || cells.ContainsKey(seaCell) || Distance(owner, seaCell) != 1)
                throw new ArgumentException("Port must reserve adjacent sea from a built Module.");
            reservedPortCells.Add(seaCell);
            blockedEdges.Add((owner, seaCell));
            blockedEdges.Add((seaCell, owner));
            Publish();
        }
        public bool IsPortReserved(Vector2Int position) => reservedPortCells.Contains(position);
        public string BuildFailure(Vector2Int position, Pipeline pipelines)
        {
            if (!BuildingPipelines.IsStandard(pipelines)) return "Select exactly 3 of F / W / M / E.";
            if (cells.ContainsKey(position)) return "Occupied: a Module already exists here.";
            if (reservedPortCells.Contains(position)) return "Port clearance: this sea cell / edge is reserved.";
            bool attached = false;
            foreach (var direction in Directions)
                if (cells.ContainsKey(position + direction) && !blockedEdges.Contains((position, position + direction))) attached = true;
            if (!attached) return "Must attach to an existing sea-facing orthogonal edge.";
            if (Storage.Get(ResourceKind.ModuleCore) < CoreCost) return "Not enough Module Core.";
            if (Storage.Get(ResourceKind.Wood) < WoodCost) return "Not enough Wood.";
            return "";
        }
        public bool TryBuild(Vector2Int position, Pipeline pipelines, out string reason)
        {
            reason = BuildFailure(position, pipelines);
            if (reason.Length != 0) return false;
            // No callbacks until both stock and physical identity are committed.
            Storage.DebitModuleConstruction();
            cells.Add(position, new ModuleCell(position, pipelines, ModuleBuilding.None));
            Revision++;
            Storage.PublishConstructionPayment();
            Changed?.Invoke();
            return true;
        }
        public bool TryConfigure(Vector2Int position, Pipeline pipelines, out string reason)
        {
            if (!cells.TryGetValue(position, out var cell)) { reason = "No Module at this location."; return false; }
            if (!BuildingPipelines.Allows(cell.Building, pipelines))
            { reason = "Requires 3/4 including building INPUT + OUTPUT: " + (BuildingPipelines.Inputs(cell.Building) | BuildingPipelines.Outputs(cell.Building)); return false; }
            cell.Pipelines = pipelines;
            Publish(); reason = ""; return true;
        }
        public bool TryOccupy(Vector2Int position, ModuleBuilding building, out string reason)
        {
            if (!cells.TryGetValue(position, out var cell) || cell.Building != ModuleBuilding.None)
            { reason = "Missing or occupied Module."; return false; }
            if (building == ModuleBuilding.None || !Enum.IsDefined(typeof(ModuleBuilding), building))
            { reason = "Choose a valid building."; return false; }
            var pipelines = BuildingPipelines.ForPlacement(building, cell.Pipelines);
            if (!BuildingPipelines.Allows(building, pipelines))
            { reason = "Building requirements exceed standard Module pipelines."; return false; }
            cell.Pipelines = pipelines;
            cell.Building = building; Publish(); reason = ""; return true;
        }
        public void SetTestSupply(Vector2Int position, Pipeline supply)
        {
            if (((int)supply & ~(int)Pipeline.All) != 0) throw new ArgumentException("Only F/W/M/E.");
            if (!cells.TryGetValue(position, out var cell)) throw new ArgumentException("Missing source Module.");
            cell.TestSupply = supply; Publish();
        }
        public bool AdjacentConnection(Vector2Int a, Vector2Int b, Pipeline type) =>
            IsSingle(type) && Distance(a, b) == 1 && !blockedEdges.Contains((a, b))
            && cells.TryGetValue(a, out var first) && first.Supports(type)
            && cells.TryGetValue(b, out var second) && second.Supports(type);
        internal static bool IsSingle(Pipeline type) => type == Pipeline.F || type == Pipeline.W || type == Pipeline.M || type == Pipeline.E;
        private static int Distance(Vector2Int a, Vector2Int b) => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        private void Publish() { Revision++; Changed?.Invoke(); }
    }
}
