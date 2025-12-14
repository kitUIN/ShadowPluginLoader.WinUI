using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace ShadowPluginLoader.WinUI.Helpers;

/// <summary>
/// HttpHelper
/// </summary>
public class BaseHttpHelper
{
    /// <summary>
    /// Lazy, thread-safe singleton instance
    /// </summary>
    private static readonly Lazy<BaseHttpHelper> InnerInstance = new(() => new BaseHttpHelper(), LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// 获取单例实例（线程安全、惰性初始化）
    /// </summary>
    public static BaseHttpHelper Instance => InnerInstance.Value;

    /// <summary>
    /// 私有构造函数，防止外部直接实例化
    /// </summary>
    protected BaseHttpHelper()
    {
        // 初始化 HttpClient（默认不使用代理）
        Client = new HttpClient(new HttpClientHandler()) { Timeout = TimeSpan.FromSeconds(100) };
    }

    /// <summary>
    /// HttpClient 可以在运行时被替换（例如切换代理），因此不是 readonly
    /// </summary>
    protected HttpClient Client;
    private readonly object _clientLock = new();
    /// <summary>
    /// Proxy
    /// </summary>
    protected IWebProxy? Proxy;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    /// <summary>
    /// 创建HttpRequestMessage
    /// </summary>
    /// <param name="method"></param>
    /// <param name="url"></param>
    /// <param name="headers"></param>
    /// <returns></returns>
    public HttpRequestMessage CreateRequestMessage(HttpMethod method, string url,
        Dictionary<string, string>? headers = null)
    {
        var httpRequestMessage = new HttpRequestMessage(method, url);
        // 默认接受 JSON
        httpRequestMessage.Headers.Accept.Clear();
        httpRequestMessage.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        if (headers == null) return httpRequestMessage;
        foreach (var header in headers.Where(header => !string.IsNullOrEmpty(header.Key) && !string.IsNullOrEmpty(header.Value)))
        {
            try
            {
                httpRequestMessage.Headers.Add(header.Key, header.Value);
            }
            catch
            {
                // 忽略无法添加为 Header 的键（例如 Content headers），调用方应通过 Content 设置
            }
        }

        return httpRequestMessage;
    }

    /// <summary>
    /// GET 并将 JSON 反序列化为 T
    /// </summary>
    public async Task<T?> GetAsync<T>(string url, Dictionary<string, string>? query = null, Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // 支持可选的 query 参数
            var finalUrl = AppendQueryToUrl(url, query);
            using var request = CreateRequestMessage(HttpMethod.Get, finalUrl, headers);
            Log.Debug("[GET] Sending request to {Url}", url);

            // 抓取当前 HttpClient 引用，避免在重置时发生竞争
            var client = Client;
            using var response = await client.SendAsync(request, cancellationToken);
            var respText = await response.Content.ReadAsStringAsync(cancellationToken);

            Log.Debug("[GET] Response from {Url}: {Resp}", url, respText);
            // 记录重要信息
            var respLen = respText.Length;
            Log.Information("[GET] {Url} -> {StatusCode}, length={Len}", url, response.StatusCode, respLen);

            response.EnsureSuccessStatusCode();

            // 如果响应为空，则返回默认值
            if (string.IsNullOrWhiteSpace(respText)) return default;
            return JsonSerializer.Deserialize<T>(respText, _jsonOptions);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "GetAsync failed for {Url}", url);
            throw;
        }
    }

    // 将查询参数正确追加到 URL（处理已有 query 并对键/值进行编码）
    private static string AppendQueryToUrl(string url, Dictionary<string, string>? query)
    {
        if (query == null || query.Count == 0) return url;

        try
        {
            var ub = new UriBuilder(url);
            // ub.Query 带前导 '?', 去掉
            var existing = ub.Query;
            if (!string.IsNullOrEmpty(existing) && existing.StartsWith("?")) existing = existing.Substring(1);

            var parts = new List<string>();
            if (!string.IsNullOrEmpty(existing)) parts.Add(existing);
            foreach (var kv in query)
            {
                var k = Uri.EscapeDataString(kv.Key);
                var v = Uri.EscapeDataString(kv.Value);
                parts.Add($"{k}={v}");
            }

            ub.Query = string.Join("&", parts);
            return ub.Uri.ToString();
        }
        catch
        {
            // fallback: 直接拼接
            var sb = new StringBuilder(url);
            sb.Append(url.Contains("?") ? "&" : "?");
            sb.Append(string.Join("&", query.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}")));
            return sb.ToString();
        }
    }

    /// <summary>
    /// POST JSON 并将响应反序列化为 TResponse
    /// </summary>
    public async Task<TResponse?> PostJsonAsync<TRequest, TResponse>(string url, TRequest payload, Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
    {
        try
        {
            using var request = CreateRequestMessage(HttpMethod.Post, url, headers);
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            Log.Debug("[POST] Sending to {Url}, payload: {Payload}", url, json);

            var client = Client;
            using var response = await client.SendAsync(request, cancellationToken);
            var respText = await response.Content.ReadAsStringAsync(cancellationToken);

            Log.Debug("[POST] Response from {Url}: {Resp}", url, respText);
            var respLen = respText.Length;
            Log.Information("[POST] {Url} -> {StatusCode}, response-length={Len}", url, response.StatusCode, respLen);

            response.EnsureSuccessStatusCode();

            if (string.IsNullOrWhiteSpace(respText)) return default;
            return JsonSerializer.Deserialize<TResponse>(respText, _jsonOptions);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "PostJsonAsync failed for {Url}", url);
            throw;
        }
    }

    /// <summary>
    /// 下载文件并返回字节数组
    /// </summary>
    public async Task<byte[]> GetFileAsync(string url, Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
    {
        try
        {
            using var request = CreateRequestMessage(HttpMethod.Get, url, headers);
            Log.Debug("[GET FILE] Downloading from {Url}", url);

            var client = Client;
            using var response = await client.SendAsync(request, cancellationToken);
            var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            var len = bytes.Length;
            Log.Information("[GET FILE] {Url} downloaded {Len} bytes", url, len);
            Log.Debug("[GET FILE] First 128 bytes (hex) of {Url}: {Preview}", url, BitConverter.ToString(bytes, 0, Math.Min(128, len)));

            response.EnsureSuccessStatusCode();
            return bytes;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "GetFileAsync failed for {Url}", url);
            throw;
        }
    }

    /// <summary>
    /// 下载并保存到磁盘
    /// </summary>
    public async Task SaveFileAsync(string url, string destPath, Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var bytes = await GetFileAsync(url, headers, cancellationToken);

            var dir = Path.GetDirectoryName(destPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

            await File.WriteAllBytesAsync(destPath, bytes, cancellationToken);

            Log.Information("[SAVE FILE] Saved {Url} -> {Path} (len={Len})", url, destPath, bytes.Length);
            Log.Debug("[SAVE FILE] Saved file from {Url} to {Path}", url, destPath);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "SaveFileAsync failed for {Url} -> {Path}", url, destPath);
            throw;
        }
    }

    /// <summary>
    /// 配置代理：传入 proxyUrl 设置代理，传入 null 则取消代理。
    /// 支持用户名/密码和是否使用默认凭据。
    /// </summary>
    public void ConfigureProxy(string? proxyUrl, string? username = null, string? password = null, bool bypassOnLocal = false, bool useDefaultCredentials = false)
    {
        IWebProxy? proxy = null;
        if (!string.IsNullOrWhiteSpace(proxyUrl))
        {
            var wp = new WebProxy(proxyUrl) { BypassProxyOnLocal = bypassOnLocal };
            if (!string.IsNullOrEmpty(username)) wp.Credentials = new NetworkCredential(username, password ?? string.Empty);
            proxy = wp;
        }

        ConfigureProxy(proxy, useDefaultCredentials);
    }

    /// <summary>
    /// 配置代理（IWebProxy 形式）。线程安全，会重建 HttpClient 并替换当前实例。
    /// </summary>
    public void ConfigureProxy(IWebProxy? proxy, bool useDefaultCredentials = false)
    {
        lock (_clientLock)
        {
            Proxy = proxy;

            var old = Client;
            var handler = new HttpClientHandler();
            if (proxy != null)
            {
                handler.Proxy = proxy;
                handler.UseProxy = true;
                handler.UseDefaultCredentials = useDefaultCredentials;
            }
            else
            {
                handler.UseProxy = false;
            }

            Client = new HttpClient(handler) { Timeout = old.Timeout };
            try
            {
                old.Dispose();
            }
            catch
            {
                // 忽略 dispose 异常
            }

            Log.Information("[PROXY] Configured proxy: {Proxy}, UseDefaultCredentials={UseDefaultCredentials}", proxy?.ToString() ?? "none", useDefaultCredentials);
            Log.Debug("[PROXY] Proxy object details: {@Proxy}", proxy);
        }
    }

    /// <summary>
    /// 获取当前代理的 ToString() 表示，或 null 表示未配置代理
    /// </summary>
    public string? GetCurrentProxy() => Proxy?.ToString();
}