using Newtonsoft.Json;

namespace PresenceBot.Integration.Router.Models;

public record GetClientListResponse(
    IDictionary<string, string> Clients
)
{
    public record Client(
        [property: JsonProperty("name")]
        string Name,
        [property: JsonProperty("nickName")]
        string NickName,
        [property: JsonProperty("ip")]
        string Ip,
        [property: JsonProperty("mac")]
        string Mac,
        [property: JsonProperty("ipMethod")]
        string IpMethod
        // [property: JsonProperty("type")]
        // string Type,
        // [property: JsonProperty("defaultType")]
        // string DefaultType,
        // [property: JsonProperty("from")]
        // string From,
        // [property: JsonProperty("macRepeat")]
        // string MacRepeat,
        // [property: JsonProperty("isGateway")]
        // string IsGateway,
        // [property: JsonProperty("isWebServer")]
        // string IsWebServer,
        // [property: JsonProperty("isPrinter")]
        // string IsPrinter,
        // [property: JsonProperty("isITunes")]
        // string IsITunes,
        // [property: JsonProperty("dpiType")]
        // string DpiType,
        // [property: JsonProperty("dpiDevice")]
        // string DpiDevice,
        // [property: JsonProperty("vendor")]
        // string Vendor,
        // [property: JsonProperty("isWL")]
        // string IsWl,
        // [property: JsonProperty("isOnline")]
        // string IsOnline,
        // [property: JsonProperty("ssid")]
        // string Ssid,
        // [property: JsonProperty("isLogin")]
        // string IsLogin,
        // [property: JsonProperty("opMode")]
        // string OpMode,
        // [property: JsonProperty("rssi")]
        // string Rssi,
        // [property: JsonProperty("curTx")]
        // string CurTx,
        // [property: JsonProperty("curRx")]
        // string CurRx,
        // [property: JsonProperty("totalTx")]
        // string TotalTx,
        // [property: JsonProperty("totalRx")]
        // string TotalRx,
        // [property: JsonProperty("wlConnectTime")]
        // string WlConnectTime,
        // [property: JsonProperty("group")]
        // string Group,
        // [property: JsonProperty("callback")]
        // string Callback,
        // [property: JsonProperty("keeparp")]
        // string Keeparp,
        // [property: JsonProperty("qosLevel")]
        // string QosLevel,
        // [property: JsonProperty("wtfast")]
        // string Wtfast,
        // [property: JsonProperty("internetMode")]
        // string InternetMode,
        // [property: JsonProperty("internetState")]
        // string InternetState
    );
};