using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ScoreRanks.Patches
{
    public enum PositionId
    {
        P1Oni,
        P1Ura,
        P2Oni,
        P2Ura,
    }

    public enum PositionIdPosition
    {
        P1OniSelected,
        P1OniUnselected,
        P1UraSelected,
        P1UraUnselected,
        P2OniSelected,
        P2OniUnselected,
        P2UraSelected,
        P2UraUnselected,
    }

    public struct ScoreRankPosition
    {
        public Vector2 Position = Vector2.zero;
        public Vector2 Scale = Vector2.one;

        public ScoreRankPosition(Vector2 position, Vector2 scale)
        {
            Position = position;
            Scale = scale;
        }
    }

    public struct ScoreRankData
    {
        //DataConst.CrownType crown, EnsoData.EnsoLevelType level
        public int PlayerNum; 
        public ScoreRank Rank;
        public EnsoData.EnsoLevelType Level;
        public PositionId CrownId { get { return GetPositionId(); } }

        public ScoreRankData(int playerNum, ScoreRank rank, EnsoData.EnsoLevelType level)
        {
            PlayerNum = playerNum;
            Rank = rank;
            Level = level;
        }

        PositionId GetPositionId()
        {
            if (Level == EnsoData.EnsoLevelType.Ura)
            {
                if (PlayerNum == 0)
                {
                    return PositionId.P1Ura;
                }
                else
                {
                    return PositionId.P2Ura;
                }
            }
            else
            {
                if (PlayerNum == 0)
                {
                    return PositionId.P1Oni;
                }
                else
                {
                    return PositionId.P2Oni;
                }
            }
        }
    }

    internal class PlayerScoreRankPositions
    {
        public static ScoreRankPosition GetScoreRankPosition(int playerNo, bool isUra, bool isSelected, float scale = 1f)
        {
            if (playerNo == 0)
            {
                if (!isUra)
                {
                    if (isSelected)
                    {
                        return GetScoreRankPosition(PositionIdPosition.P1OniSelected, scale);

                    }
                    else
                    {
                        return GetScoreRankPosition(PositionIdPosition.P1OniUnselected, scale);

                    }
                }
                else
                {
                    if (isSelected)
                    {
                        return GetScoreRankPosition(PositionIdPosition.P1UraSelected, scale);

                    }
                    else
                    {
                        return GetScoreRankPosition(PositionIdPosition.P1UraUnselected, scale);
                    }
                }
            }
            else if (playerNo == 1)
            {
                if (!isUra)
                {
                    if (isSelected)
                    {
                        return GetScoreRankPosition(PositionIdPosition.P2OniSelected, scale);

                    }
                    else
                    {
                        return GetScoreRankPosition(PositionIdPosition.P2OniUnselected, scale);

                    }
                }
                else
                {
                    if (isSelected)
                    {
                        return GetScoreRankPosition(PositionIdPosition.P2UraSelected, scale);

                    }
                    else
                    {
                        return GetScoreRankPosition(PositionIdPosition.P2UraUnselected, scale);
                    }
                }
            }
            else
            {
                return new ScoreRankPosition(
                            new Vector2(0, 0),
                            new Vector2(1f, 1f));
            }
        }

        public static ScoreRankPosition GetScoreRankPosition(PositionIdPosition crown, float scale = 1f)
        {
            float selectedP1X = -451f * scale;
            float selectedOniY = 10f * scale;
            float selectedP2X = 445f * scale;
            float selectedUraY = -42f * scale;
            float selectedScale = 1f * scale;

            float unselectedP1X = -363f * scale;
            float unselectedOniY = 14f * scale;
            float unselectedP2X = 407f * scale;
            float unselectedUraY = -16f * scale;
            float unselectedScale = 0.65f * scale;

            switch (crown)
            {
                case PositionIdPosition.P1OniSelected:
                    return new ScoreRankPosition(
                            new Vector2(selectedP1X, selectedOniY),
                            new Vector2(selectedScale, selectedScale));
                case PositionIdPosition.P1OniUnselected:
                    return new ScoreRankPosition(
                            new Vector2(unselectedP1X, unselectedOniY),
                            new Vector2(unselectedScale, unselectedScale));
                case PositionIdPosition.P1UraSelected:
                    return new ScoreRankPosition(
                            new Vector2(selectedP1X, selectedUraY),
                            new Vector2(selectedScale, selectedScale));
                case PositionIdPosition.P1UraUnselected:
                    return new ScoreRankPosition(
                            new Vector2(unselectedP1X, unselectedUraY),
                            new Vector2(unselectedScale, unselectedScale));
                case PositionIdPosition.P2OniSelected:
                    return new ScoreRankPosition(
                            new Vector2(selectedP2X, selectedOniY),
                            new Vector2(selectedScale, selectedScale));
                case PositionIdPosition.P2OniUnselected:
                    return new ScoreRankPosition(
                            new Vector2(unselectedP2X, unselectedOniY),
                            new Vector2(unselectedScale, unselectedScale));
                case PositionIdPosition.P2UraSelected:
                    return new ScoreRankPosition(
                            new Vector2(selectedP2X, selectedUraY),
                            new Vector2(selectedScale, selectedScale));
                case PositionIdPosition.P2UraUnselected:
                    return new ScoreRankPosition(
                            new Vector2(unselectedP2X, unselectedUraY),
                            new Vector2(unselectedScale, unselectedScale));
                default:
                    return new ScoreRankPosition(
                            new Vector2(0, 0),
                            new Vector2(1f, 1f));
            }
        }

        public static ScoreRankPosition GetScoreRankPosition(PositionId id, bool isSelected, float scale = 1f)
        {
            switch (id)
            {
                case PositionId.P1Oni: return isSelected ? GetScoreRankPosition(PositionIdPosition.P1OniSelected, scale) : GetScoreRankPosition(PositionIdPosition.P1OniUnselected, scale);
                case PositionId.P1Ura: return isSelected ? GetScoreRankPosition(PositionIdPosition.P1UraSelected, scale) : GetScoreRankPosition(PositionIdPosition.P1UraUnselected, scale);
                case PositionId.P2Oni: return isSelected ? GetScoreRankPosition(PositionIdPosition.P2OniSelected, scale) : GetScoreRankPosition(PositionIdPosition.P2OniUnselected, scale);
                case PositionId.P2Ura: return isSelected ? GetScoreRankPosition(PositionIdPosition.P2UraSelected, scale) : GetScoreRankPosition(PositionIdPosition.P2UraUnselected, scale);
            }

            return new ScoreRankPosition(new Vector2(0, 0),
                                     new Vector2(1f, 1f));
        }
    }
}
