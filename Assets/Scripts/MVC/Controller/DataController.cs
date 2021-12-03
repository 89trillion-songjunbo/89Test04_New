using MVC.Model;
using UnityEngine;

namespace MVC.Controller
{
    public class DataController : BaseAPI
    {
        public UILeaderBoardModel Model1 { get; set; }

        public DataController(GameObject gameObject) : base(gameObject)
        {
            ForceRequest = false;
        }

        private readonly string myToken =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1aWQiOiI0MzY4NjY1MjcifQ.drFj2OtLEjgE452sgtHPG73xU-yQ-OXvbz4Utxl2M1k";

        public void Request()
        {
            var clientBuilder = CreateHttpClientBuilder(myToken);
            SendRequest(clientBuilder);
        }

        private HttpClientBuilder CreateHttpClientBuilder(string token, int seasonId = 18, int page = 1,
            bool isForceRequest = false)
        {
            ForceRequest = isForceRequest;

            const string path = "http://api-s2.artofwarconquest.com/admin/rankList";
            var httpClientBuilder = new HttpClientBuilder(path)
                .Param("type", 1)
                .Param("season", seasonId)
                .Param("page", page)
                .Param("token", token)
                .Method(HttpMethod.Get);

            if (seasonId > 0)
            {
                httpClientBuilder.Param("seasonId", seasonId);
            }

            return httpClientBuilder;
        }
    }
}