using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public class Res<T>
{
    public bool ok;
    public T data;
}

public class DBHelper<B, R>
{
    private HttpClient client = new();
    private string route;
    private StringContent content;

    public DBHelper(string route, B body)
    {
        Debug.Log(Constants.Instance.DBUri());
        client.BaseAddress = new Uri(Constants.Instance.DBUri());
        string jsonString = JsonConvert.SerializeObject(body);
        content = new StringContent(jsonString, Encoding.UTF8, "application/json");
        this.route = route;
    }

    public Res<R> GetResponse()
    {
        var response = client.PostAsync(route, content).Result;

        if (!response.IsSuccessStatusCode)
        {
            Debug.LogWarning("Unexpected DB Error");
            return new Res<R> { ok = false, data = default };
        }

        var responseContent = response.Content.ReadAsStringAsync().Result;
        var res = JsonConvert.DeserializeObject<R>(responseContent);
        return new Res<R> { ok = true, data = res };
    }
}
