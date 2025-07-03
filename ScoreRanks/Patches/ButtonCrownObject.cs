using SongSelect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ScoreRanks.Patches
{
    internal class ButtonCrownObject
    {
        public Dictionary<PositionId, GameObject> GameObjects = new Dictionary<PositionId, GameObject>();
        bool isSelected;

        SongSelectKanban Parent;
        SongSelectManager.Song Song => Parent.displayingSong;

        public ButtonCrownObject(SongSelectKanban parent)
        {
            Parent = parent;
            isSelected = parent.name == "Kanban1";
            InitializeCrownGameObjects(parent.gameObject);
        }


        public bool IsInitialized()
        {
            foreach (var gameObjects in GameObjects)
            {
                if (gameObjects.Value == null)
                {
                    return false;
                }
            }

            return true;
        }

        void InitializeCrownGameObjects(GameObject parent)
        {
            GameObject crownParent = new GameObject("ScoreRank");
            crownParent.transform.SetParent(parent.transform);
            crownParent.transform.localPosition = Vector2.zero;
            InitializeCrownGameObject(PositionId.P1Oni, crownParent);
            InitializeCrownGameObject(PositionId.P1Ura, crownParent);
            InitializeCrownGameObject(PositionId.P2Oni, crownParent);
            InitializeCrownGameObject(PositionId.P2Ura, crownParent);
        }

        void InitializeCrownGameObject(PositionId crownId, GameObject parent)
        {
            GameObject crownObj = new GameObject(crownId.ToString());
            crownObj.transform.SetParent(parent.transform);
            ScoreRankPosition pos = PlayerScoreRankPositions.GetScoreRankPosition(crownId, isSelected);
            crownObj.transform.localPosition = pos.Position;
            crownObj.transform.localScale = pos.Scale;

            GameObject crown = new GameObject("ScoreRank");
            crown.transform.SetParent(crownObj.transform);
            var crownImage = crown.AddComponent<Image>();
            crownImage.raycastTarget = false;
            var crownRect = crown.GetComponent<RectTransform>();
            crownRect.localPosition = new Vector2(0, 0);
            crownRect.localScale = new Vector2(1f, 1f);

            GameObject diffObj = new GameObject("Difficulty");
            diffObj.transform.SetParent(crownObj.transform);
            var diffImage = diffObj.AddComponent<Image>();
            diffImage.raycastTarget = false;
            var diffRect = diffObj.GetComponent<RectTransform>();
            diffRect.localPosition = new Vector2(10, -10);
            diffRect.localScale = new Vector2(0.7f, 0.7f);

            crownObj.SetActive(false);

            GameObjects.Add(crownId, crownObj);
        }

        internal IEnumerator ChangeScoreRanks()
        {
            //Logger.Log("musicInfo.Id:" + musicInfo.Id);
            while (!SpriteInitialization.IsInitialized())
            {
                yield return new WaitForEndOfFrame();
            }

            // I need to test this when I have a controller available
            var numPlayers = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.playerNum;
            //Logger.Log(numPlayers.ToString(), LogType.Debug);

            var musicInfo = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.MusicData.GetInfoById(Song.Id);

            for (int i = 0; i < 1; i++)
            {
                var scores = (i == 0 ? Song.HighScores : Song.HighScores2P);
                if (Song.Stars.Length >= (int)EnsoData.EnsoLevelType.Ura &&
                    Song.Stars[(int)EnsoData.EnsoLevelType.Ura] == 0)
                {
                    ChangeScoreRank(new ScoreRankData(i, ScoreRank.None, EnsoData.EnsoLevelType.Ura));
                }
                else
                {
                    var diffIndex = (int)EnsoData.EnsoLevelType.Ura;
                    var scoreRank = ScoreRankUtility.GetScoreRank(scores[diffIndex].hiScoreRecordInfos.score, musicInfo.Scores[diffIndex]);
                    ChangeScoreRank(new ScoreRankData(i, scoreRank, EnsoData.EnsoLevelType.Ura));
                }

                bool crownFound = false;
                for (EnsoData.EnsoLevelType j = EnsoData.EnsoLevelType.Mania; j >= EnsoData.EnsoLevelType.Easy; j--)
                {
                    if (scores[(int)j].crown != DataConst.CrownType.None)
                    {
                        crownFound = true;
                        var scoreRank = ScoreRankUtility.GetScoreRank(scores[(int)j].hiScoreRecordInfos.score, musicInfo.Scores[(int)j]);
                        ChangeScoreRank(new ScoreRankData(i, scoreRank, j));
                        break;
                    }
                }

                if (!crownFound)
                {
                    ChangeScoreRank(new ScoreRankData(i, ScoreRank.None, EnsoData.EnsoLevelType.Num));
                }
            }
        }

        void ChangeScoreRank(ScoreRankData data)
        {
            var crownId = data.CrownId;
            if (data.Rank == ScoreRank.None)
            {
                //if (CrownGameObjects.ContainsKey(crownId))
                {
                    GameObjects[crownId].SetActive(false);
                }
                return;
            }

            GameObjects[crownId].SetActive(true);

            var crownObj = GameObjects[crownId].transform.FindChild("ScoreRank");
            var crownImage = crownObj.GetComponent<Image>();
            crownImage.sprite = AssetUtility.LoadSprite(ScoreRankUtility.GetSpriteFilePath(data.Rank, ScoreRankSpriteVersion.Small));
            // I'm not sure if we need to set the sizeDelta each time
            var crownRect = crownObj.GetComponent<RectTransform>();
            crownRect.sizeDelta = new Vector2(crownImage.sprite.rect.width, crownImage.sprite.rect.height);

            var diffObj = GameObjects[crownId].transform.FindChild("Difficulty");
            diffObj.gameObject.SetActive(true);
            var diffImage = diffObj.GetComponent<Image>();
            diffImage.sprite = SpriteInitialization.DifficultySprites[data.Level];
            // I'm not sure if we need to set the sizeDelta each time
            var diffRect = diffObj.GetComponent<RectTransform>();
            diffRect.sizeDelta = new Vector2(diffImage.sprite.rect.width, diffImage.sprite.rect.height);
        }


        public void ExpandCrowns()
        {
            Plugin.Instance.StartCoroutine(MoveCrown(PositionId.P1Oni, PositionIdPosition.P1OniUnselected, PositionIdPosition.P1OniSelected, 0.125f));
            Plugin.Instance.StartCoroutine(MoveCrown(PositionId.P1Ura, PositionIdPosition.P1UraUnselected, PositionIdPosition.P1UraSelected, 0.125f));

            Plugin.Instance.StartCoroutine(MoveCrown(PositionId.P2Oni, PositionIdPosition.P2OniUnselected, PositionIdPosition.P2OniSelected, 0.125f));
            Plugin.Instance.StartCoroutine(MoveCrown(PositionId.P2Ura, PositionIdPosition.P2UraUnselected, PositionIdPosition.P2UraSelected, 0.125f));
        }

        public void ExpandCrownsImmediate()
        {
            MoveCrownImmediate(PositionId.P1Oni, PositionIdPosition.P1OniSelected, PositionIdPosition.P1OniSelected);
            MoveCrownImmediate(PositionId.P1Ura, PositionIdPosition.P1UraSelected, PositionIdPosition.P1UraSelected);
                                                 
            MoveCrownImmediate(PositionId.P2Oni, PositionIdPosition.P2OniSelected, PositionIdPosition.P2OniSelected);
            MoveCrownImmediate(PositionId.P2Ura, PositionIdPosition.P2UraSelected, PositionIdPosition.P2UraSelected);
        }

        public void ShrinkCrowns()
        {
            Plugin.Instance.StartCoroutine(MoveCrown(PositionId.P1Oni, PositionIdPosition.P1OniSelected, PositionIdPosition.P1OniUnselected, 0.125f));
            Plugin.Instance.StartCoroutine(MoveCrown(PositionId.P1Ura, PositionIdPosition.P1UraSelected, PositionIdPosition.P1UraUnselected, 0.125f));

            Plugin.Instance.StartCoroutine(MoveCrown(PositionId.P2Oni, PositionIdPosition.P2OniSelected, PositionIdPosition.P2OniUnselected, 0.125f));
            Plugin.Instance.StartCoroutine(MoveCrown(PositionId.P2Ura, PositionIdPosition.P2UraSelected, PositionIdPosition.P2UraUnselected, 0.125f));
        }

        public void ShrinkCrownsImmediate()
        {
            MoveCrownImmediate(PositionId.P1Oni, PositionIdPosition.P1OniUnselected, PositionIdPosition.P1OniUnselected);
            MoveCrownImmediate(PositionId.P1Ura, PositionIdPosition.P1UraUnselected, PositionIdPosition.P1UraUnselected);
                                                 
            MoveCrownImmediate(PositionId.P2Oni, PositionIdPosition.P2OniUnselected, PositionIdPosition.P2OniUnselected);
            MoveCrownImmediate(PositionId.P2Ura, PositionIdPosition.P2UraUnselected, PositionIdPosition.P2UraUnselected);
        }

        IEnumerator MoveCrown(PositionId crown, PositionIdPosition start, PositionIdPosition end, float duration)
        {
            if (GameObjects.ContainsKey(crown))
            {
                GameObject crownObj = GameObjects[crown];

                float elapsedTime = 0f;
                ScoreRankPosition startPos = PlayerScoreRankPositions.GetScoreRankPosition(start);
                ScoreRankPosition endPos = PlayerScoreRankPositions.GetScoreRankPosition(end);

                while (elapsedTime < duration)
                {
                    crownObj.transform.localPosition = Vector2.Lerp(startPos.Position, endPos.Position, elapsedTime / duration);
                    crownObj.transform.localScale = Vector2.Lerp(startPos.Scale, endPos.Scale, elapsedTime / duration);

                    elapsedTime += Time.deltaTime;

                    yield return new WaitForEndOfFrame();
                }

                crownObj.transform.localPosition = endPos.Position;
                crownObj.transform.localScale = endPos.Scale;
            }
        }

        void MoveCrownImmediate(PositionId crown, PositionIdPosition start, PositionIdPosition end)
        {
            if (GameObjects.ContainsKey(crown))
            {
                GameObject crownObj = GameObjects[crown];

                ScoreRankPosition startPos = PlayerScoreRankPositions.GetScoreRankPosition(start);
                ScoreRankPosition endPos = PlayerScoreRankPositions.GetScoreRankPosition(end);

                crownObj.transform.localPosition = endPos.Position;
                crownObj.transform.localScale = endPos.Scale;
            }
        }
    }
}
