using ShadowPluginLoader.Attributes;

namespace ShadowExample.Plugin.Emoji;

[ShadowSettingClass(Container = "ShadowExample.Plugin.Emoji", ClassName = "EmojiSetting")]
public enum BikaConfigKey
{
    [ShadowSetting(typeof(int), "1", "Api分流")]
    ApiShunt,

    [ShadowSetting(typeof(bool), "true", "登陆后记住我")]
    RememberMe,

    [ShadowSetting(typeof(string), "tt",comment: "测试名称")]
    TestName,
    
    [ShadowSetting(typeof(string), "Temp",  "测试名称", true)]
    TempPath,
}