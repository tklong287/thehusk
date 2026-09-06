using System.Collections.Generic;
using UnityEngine;

namespace Husk
{
    public enum PipelineState { Unsupported, Unsupplied, Supplied }

    // Four independent connected-component maps, recomputed only after a layout/source change.
    public sealed class ModuleNetwork
    {
        public static IReadOnlyList<Pipeline> Types { get; } = System.Array.AsReadOnly(new[] { Pipeline.F, Pipeline.W, Pipeline.M, Pipeline.E });
        private readonly ModuleLayout layout;
        private readonly Dictionary<(Vector2Int, Pipeline), int> components = new();
        private readonly HashSet<(Pipeline, int)> supplied = new();
        private int revision = -1;
        public ModuleNetwork(ModuleLayout layout) { this.layout = layout; }
        private void Refresh()
        {
            if (revision == layout.Revision) return;
            components.Clear(); supplied.Clear();
            var queue = new Queue<Vector2Int>();
            foreach (var type in Types)
            {
                int id = 0;
                foreach (var pair in layout.Cells)
                {
                    if (!pair.Value.Supports(type) || components.ContainsKey((pair.Key, type))) continue;
                    id++;
                    components.Add((pair.Key, type), id); queue.Enqueue(pair.Key);
                    while (queue.Count > 0)
                    {
                        var position = queue.Dequeue();
                        if ((layout.Cells[position].TestSupply & type) != 0) supplied.Add((type, id));
                        foreach (var direction in ModuleLayout.Directions)
                        {
                            var next = position + direction;
                            if (components.ContainsKey((next, type)) || !layout.AdjacentConnection(position, next, type)) continue;
                            components.Add((next, type), id); queue.Enqueue(next);
                        }
                    }
                }
            }
            revision = layout.Revision;
        }
        public int Component(Vector2Int position, Pipeline type)
        { Refresh(); return components.TryGetValue((position, type), out int id) ? id : -1; }
        public bool Connected(Vector2Int a, Vector2Int b, Pipeline type)
        { int id = Component(a, type); return id >= 0 && id == Component(b, type); }
        public PipelineState State(Vector2Int position, Pipeline type)
        {
            int id = Component(position, type);
            return id < 0 ? PipelineState.Unsupported : supplied.Contains((type, id)) ? PipelineState.Supplied : PipelineState.Unsupplied;
        }
    }
}
