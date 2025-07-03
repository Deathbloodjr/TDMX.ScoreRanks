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
        public static ScoreRankPosition GetScoreRankPosition(int playerNo, bool isUra, bool isSelected)
        {
            if (playerNo == 0)
            {
                if (!isUra)
                {
                    if (isSelected)
                    {
                        return GetScoreRankPosition(PositionIdPosition.P1OniSelected);

                    }
                    else
                    {
                        return GetScoreRankPosition(PositionIdPosition.P1OniUnselected);

                    }
                }
                else
                {
                    if (isSelected)
                    {
                        return GetScoreRankPosition(PositionIdPosition.P1UraSelected);

                    }
                    else
                    {
                        return GetScoreRankPosition(PositionIdPosition.P1UraUnselected);
                    }
                }
            }
            else if (playerNo == 1)
            {
                if (!isUra)
                {
                    if (isSelected)
                    {
                        return GetScoreRankPosition(PositionIdPosition.P2OniSelected);

                    }
                    else
                    {
                        return GetScoreRankPosition(PositionIdPosition.P2OniUnselected);

                    }
                }
                else
                {
                    if (isSelected)
                    {
                        return GetScoreRankPosition(PositionIdPosition.P2UraSelected);

                    }
                    else
                    {
                        return GetScoreRankPosition(PositionIdPosition.P2UraUnselected);
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

        public static ScoreRankPosition GetScoreRankPosition(PositionIdPosition crown)
        {
            float selectedP1X = -451f;
            float selectedOniY = 10f;
            float selectedP2X = 445f;
            float selectedUraY = -42f;
            float selectedScale = 1f;

            float unselectedP1X = -363f;
            float unselectedOniY = 14f;
            float unselectedP2X = 407f;
            float unselectedUraY = -16f;
            float unselectedScale = 0.65f;

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

        public static ScoreRankPosition GetScoreRankPosition(PositionId id, bool isSelected)
        {
            switch (id)
            {
                case PositionId.P1Oni: return isSelected ? GetScoreRankPosition(PositionIdPosition.P1OniSelected) : GetScoreRankPosition(PositionIdPosition.P1OniUnselected);
                case PositionId.P1Ura: return isSelected ? GetScoreRankPosition(PositionIdPosition.P1UraSelected) : GetScoreRankPosition(PositionIdPosition.P1UraUnselected);
                case PositionId.P2Oni: return isSelected ? GetScoreRankPosition(PositionIdPosition.P2OniSelected) : GetScoreRankPosition(PositionIdPosition.P2OniUnselected);
                case PositionId.P2Ura: return isSelected ? GetScoreRankPosition(PositionIdPosition.P2UraSelected) : GetScoreRankPosition(PositionIdPosition.P2UraUnselected);
            }

            return new ScoreRankPosition(new Vector2(0, 0),
                                     new Vector2(1f, 1f));
        }
    }
}
