using SongSelect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreRanks.Patches
{
    public enum Kanban
    {
        None,
        Kanban1,
        Kanban2,
        Kanban3,
        Kanban4,
        Kanban5,
        Kanban6,
        Kanban7,
        Kanban8,
        Kanban9,
        Kanban10,
        Kanban11,
    }

    internal class KanbanUtility
    {
        public static Kanban KanbanToEnum(SongSelectKanban kanban)
        {
            switch (kanban.name)
            {
                case "Kanban1": return Kanban.Kanban1;
                case "Kanban2": return Kanban.Kanban2;
                case "Kanban3": return Kanban.Kanban3;
                case "Kanban4": return Kanban.Kanban4;
                case "Kanban5": return Kanban.Kanban5;
                case "Kanban6": return Kanban.Kanban6;
                case "Kanban7": return Kanban.Kanban7;
                case "Kanban8": return Kanban.Kanban8;
                case "Kanban9": return Kanban.Kanban9;
                case "Kanban10": return Kanban.Kanban10;
                case "Kanban11": return Kanban.Kanban11;
                default: return Kanban.None;
            }
        }
    }
}
