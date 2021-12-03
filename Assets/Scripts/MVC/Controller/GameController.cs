using System.Collections;
using MVC.Model;
using MVC.View;
using Tools;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Your.Namespace.Here.UniqueStringHereToAvoidNamespaceConflicts.Lists;

namespace MVC.Controller
{
    public class GameController : MonoBehaviour
    {
        private UILeaderBoardModel _uiLeaderBoardModel;

        private UILeaderBoardModel UILeaderBoardModel =>
            _uiLeaderBoardModel ?? (_uiLeaderBoardModel = new UILeaderBoardModel());

        [FormerlySerializedAs("mUiRankItemPanel")] [SerializeField]
        private RankItemPanelView mUiRankItemPanelView;

        [SerializeField] private Text mUiCountDonwTe;
        [SerializeField] private GameObject mUiMainObj;
        [SerializeField] private Button mUiStartBtn;
        [SerializeField] private Transform mUiTipsTran;
        [SerializeField] private BasicListAdapter mUiListAda;

        private void Start()
        {
            mUiListAda.EventCreateTips += ShowToast;

            mUiStartBtn.gameObject.SetActive(true);
            mUiStartBtn.onClick.AddListener(() =>
            {
                if (!UILeaderBoardModel.HaveData)
                {
                    return;
                }

                mUiMainObj.SetActive(true);
                StartCoroutine(CountDownFunc(UILeaderBoardModel.LeaderRankInfo.CountDown));
                mUiStartBtn.gameObject.SetActive(false);
            });
            mUiListAda.uiLeaderBoardModel = UILeaderBoardModel;
        }

        private int seasonId;

        private IEnumerator CountDownFunc(long endTime)
        {
            while (endTime > 0)
            {
                yield return new WaitForSeconds(1f);
                SetData(endTime);
                endTime--;
            }
        }

        private void SetData(long timer)
        {
            var day = Mathf.FloorToInt(timer / (3600 * 24) % 30);
            var hour = Mathf.FloorToInt(timer / 3600 % 24);
            var minute = Mathf.FloorToInt(timer / 60 % 60);
            var second = Mathf.FloorToInt(timer % 60);
            mUiCountDonwTe.text = $"Season {seasonId} Ranking \n Ends: {day}d {hour}h {minute}m {second}s";
        }

        public TipsPanelView tipsPanel;

        private void ShowToast(string content)
        {
            if (tipsPanel != null && tipsPanel.gameObject.activeSelf)
            {
                return;
            }

            tipsPanel.Init(content);
        }
    }

    /// <summary>
    /// 资源文件路径 及名字 不属于字段命名
    /// </summary>
    public static class Const
    {
        public const string FrontRankPath = "UI/rank_";
        public const string SegmentImgPath = "UI/Segment/arenaBadge_";
        public const string RankBgImgPath = "UI/rank list_";
        public const string RankNormalPath = "UI/rank list_normal";
    }
}