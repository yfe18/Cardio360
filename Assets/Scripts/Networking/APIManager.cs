using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public static class APIManager
{
    [Serializable]
    private struct ResponseData {
        public List<ResponseValueData> values;
        public float time_step;
        public List<float> time_values;
    }

    [Serializable]
    private struct ResponseValueData {   
        public string varname;
        public string plot_path, y_axis_path, x_axis_path;
        public RectInt plot_bbox;
        public List<float> data;
    }

    private static readonly Uri BASE_API_URL = new Uri("http://localhost:5000");

    public static IEnumerator RunModelServer(string protocol, int stoptime, Action<ModelData?, string> returnCallback,
        int drugConc = 0, int INa_IC50 = 0, int ICaL_IC50 = 0, int IKr_IC50 = 0) 
    {
        // make initial request to run model

        string queryString = "?protocol={0}&stoptime={1}";
        if (drugConc != 0) {
            queryString += "&drugConc={2}&INa_IC50={3}&ICaL_IC50={4}&IKr_IC50={5}";
        }

        queryString = String.Format(queryString, protocol, stoptime, drugConc, INa_IC50, ICaL_IC50, IKr_IC50);

        UriBuilder uriBuilder = new UriBuilder(BASE_API_URL);
        uriBuilder.Path = "ten_tuss";
        uriBuilder.Query = queryString;

        // download the graphs and build the output data
        
        using (UnityWebRequest request = UnityWebRequest.Get(uriBuilder.Uri)) {

            yield return request.SendWebRequest();

            if(request.result != UnityWebRequest.Result.Success) {
                returnCallback(null, "Unable to access API." + request.error);
                yield break;
            }

            ResponseData response = JsonUtility.FromJson<ResponseData>(request.downloadHandler.text);

            ModelData outputModelData = new ModelData(response.time_step);
            outputModelData.timeValues = response.time_values;

            UriBuilder imgUriBuilder = new UriBuilder(BASE_API_URL);

            // get x axis image for each; exploits that x axis path and image will be same for each
            imgUriBuilder.Path = response.values[0].x_axis_path;
            Texture xAxisImage = Texture2D.blackTexture; // use black texture as temp texture
            yield return DownloadImage(imgUriBuilder.Uri, (Texture texture) => {xAxisImage = texture;});

            // get plot and y axis for each
            foreach(ResponseValueData valueData in response.values) {

                Texture plotImage = Texture2D.blackTexture, yAxisImage = Texture2D.blackTexture;

                imgUriBuilder.Path = valueData.plot_path;
                yield return DownloadImage(imgUriBuilder.Uri, (Texture texture) => {plotImage = texture;});
                // TODO: add error handling to these requests
                imgUriBuilder.Path = valueData.y_axis_path;
                yield return DownloadImage(imgUriBuilder.Uri, (Texture texture) => {yAxisImage = texture;});

                ModelValue newModelValue = new ModelValue(valueData.data.ToArray(), outputModelData.timeValues.ToArray(),
                    plotImage, xAxisImage, yAxisImage, valueData.plot_bbox);

                outputModelData.modelValues[valueData.varname] = newModelValue;
            }

            returnCallback(outputModelData, "");
        }
    }

    /*private IEnumerator GetDataFromServer() {

        Dictionary<string, string> query= new Dictionary<string, string>() {
            {"protocol", "dynamic"},
            {"stoptime", "20"}
        };

        string queryString = "?";

        foreach (KeyValuePair<string, string> kv in query) {
            queryString += kv.Key + "=" + kv.Value + "&";
        }
        queryString = queryString.Remove(queryString.Length-1);
        
        UriBuilder uriBuilder = new UriBuilder(BASE_API_URL);
        uriBuilder.Path = "ten_tuss";
        uriBuilder.Query = queryString;
        
        using (UnityWebRequest request = UnityWebRequest.Get(uriBuilder.Uri)) {

            yield return request.SendWebRequest();

            ResponseData response = JsonUtility.FromJson<ResponseData>(request.downloadHandler.text);

            UriBuilder imgUriBuilder = new UriBuilder(BASE_API_URL);

            foreach(ModelValues modelValue in response.values) {
                imgUriBuilder.Path = modelValue.plot_path;
                yield return DownloadImage(imgUriBuilder.Uri, (Texture texture) => {rawImage.texture = texture;});
                imgUriBuilder.Path = modelValue.y_axis_path;
                yield return DownloadImage(imgUriBuilder.Uri, (Texture texture) => {rawImage.texture = texture;});
                //imgUriBuilder.Path = modelValue.x_axis_path;
                //yield return DownloadImage(imgUriBuilder.Uri);
            }
        }
    }*/

    private static IEnumerator DownloadImage(Uri url, Action<Texture> returnCallback) {
        using (UnityWebRequest request = UnityWebRequest.Get(url)) {
            DownloadHandlerTexture downloadHandlerTexture = new DownloadHandlerTexture();
            request.downloadHandler = downloadHandlerTexture;

            yield return request.SendWebRequest();

            returnCallback.Invoke(downloadHandlerTexture.texture);
        }
    }
}
