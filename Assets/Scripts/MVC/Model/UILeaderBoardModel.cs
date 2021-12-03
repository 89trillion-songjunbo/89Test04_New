using System;
using System.Collections.Generic;
using System.Linq;
using MVC.Controller;
using SimpleJSON;
using UnityEngine;

namespace MVC.Model
{
    public class UILeaderBoardModel
    {
        public bool HaveData { private set; get; }

        public UILeaderBoardModel()
        {
            GetDataFromHttp();
        }

        private void GetDataFromHttp()
        {
            var dataController = new DataController(new GameObject())
            {
                Model1 = this,
                OnSuccess = OnRankApiSuccess,
                OnCacheRestore = data => { Debug.Log("CacheRestore" + data); },
                OnError = EventOnError
            };
            dataController.Request();
        }

        private void OnRankApiSuccess(string info)
        {
            Debug.Log("http成功信息 = " + info);
            LeaderRankInfo = ReadData(info, true);
            HaveData = LeaderRankInfo != null;

            EventReadDataSuccess?.Invoke();
        }

        private void EventOnError(string info, int code)
        {
            Debug.LogError("http 报错 " + info);
        }

        private Action EventReadDataSuccess;

        public PersonInfo GetPersonData(string uid)
        {
            List<PersonInfo> infoLis = null;

            if (LeaderRankInfo != null)
            {
                infoLis = LeaderRankInfo.PersonInfos.Where(a => (a.Uid == uid)).ToList();
            }

            return (infoLis != null && infoLis.Count > 0) ? infoLis[0] : null;
        }

        public int GetRank(string uid)
        {
            int rank = 0;
            if (LeaderRankInfo != null)
            {
                rank = LeaderRankInfo.PersonInfos.FindIndex(item => item.Uid.Equals(uid));
            }

            return rank + 1;
        }

        public LeaderRankInfo LeaderRankInfo { get; set; }

        public void ReadData()
        {
            var json = Resources.Load<TextAsset>("json/ranklist");
            LeaderRankInfo = ReadData(json.ToString());
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <returns></returns>
        public LeaderRankInfo ReadData(string data, bool isFromHttp = false)
        {
            var root = JSON.Parse(data);

            var leaderRankInfos = new LeaderRankInfo();

            JSONNode personInfos = null;
            if (!isFromHttp)
            {
                personInfos = root["list"];
                leaderRankInfos.CountDown = root["countDown"];
                leaderRankInfos.SeasonID = root["seasonID"].AsInt;
                leaderRankInfos.SelfRank = root["selfRank"].AsInt;
            }
            else
            {
                personInfos = root["data"]["list"];
            }


            for (int i = 0; i < personInfos.Count; i++)
            {
                var personInfo = personInfos[i];

                PersonInfo info = new PersonInfo(
                    personInfo["uid"],
                    personInfo["nickName"],
                    personInfo["trophy"].AsInt
                );

                leaderRankInfos.PersonInfos.Add(info);
            }


            leaderRankInfos.PersonInfos.Sort((a, b) => b.Trophy - a.Trophy);

            HaveData = leaderRankInfos != null;

            return leaderRankInfos;
        }
    }

    public class LeaderRankInfo
    {
        public int CountDown = 2048;
        public readonly List<PersonInfo> PersonInfos = new List<PersonInfo>();
        public int SeasonID = 18;
        public int SelfRank = 1;
    }

    public class PersonInfo
    {
        public readonly string Uid;
        public string NickName;
        public int Trophy;

        public PersonInfo(string uid, string nickName, int trophy)
        {
            Uid = uid;
            NickName = nickName;
            Trophy = trophy;
        }
    }
}