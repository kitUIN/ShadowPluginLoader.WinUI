
function Read-Json  
{
    # 加载json
    param(
        [string]$json_file
    )
    return Get-Content -Raw -Path $json_file | ConvertFrom-Json
}
function Find-JsonKey
{
    # json是否含有key
    param(
        [PSObject]$Obj,
        [string]$Key
    )
    return $Obj.psobject.properties.match($Key).Count -ne 0
}
function Get-JsonValue
{
    # json是否含有key
    param(
        [PSObject]$Obj,
        [string]$Key
    )
    return $Obj.psobject.properties.Item($Key).Value
}
$input_xmlfile = "D:\VsProjects\WASDK\ShadowPluginLoader.WinUI\ShadowExample.Plugin.Emoji\ShadowExample.Plugin.Emoji.csproj"
$meta_define = "D:\VsProjects\WASDK\ShadowPluginLoader.WinUI\ShadowExample.Plugin.Emoji\meta.d.json"
$default_keys = @("path","type","initName")
$default__keys = @("_MetaClass","_Prefix")

$proj = [xml](Get-Content $input_xmlfile)
$meta_d = Read-Json $meta_define
$proj.SelectSingleNode("//Project/PropertyGroup/PackageId").InnerText
$res_json = @{}
if($meta_d.psobject.properties.name.Count -eq 0){
    Write-Host "[Pre Build] [ERROR] | meta.d.json Is Empty" -ForegroundColor red
    exit 1
}
foreach($_key in $default__keys){
    if(!(Find-JsonKey -Obj $meta_d -Key $_key)){
        Write-Host "[Pre Build] [ERROR] | meta.d.json | root.$_key Not Found" -ForegroundColor red
    }
}
$prefix = (Get-JsonValue -Obj $meta_d -Key "_Prefix") + "."
foreach ($item in $meta_d.psobject.properties.name)
{
    if(!$item.StartsWith("_")){
        $f = Get-JsonValue -Obj $meta_d -Key $item 
        foreach($key in $default_keys){
            if(!(Find-JsonKey -Obj $f -Key $key)){
                Write-Host "[Pre Build] [ERROR] | meta.d.json | $item.$key Not Found" -ForegroundColor red
            }
        }
        if(Find-JsonKey -Obj $f -Key "required"){
            $required = Get-JsonValue -Obj $f -Key "required"
            if($required){

            }
        }
        $j_regex = $null
        if(Find-JsonKey -Obj $f -Key "regex"){
            $j_regex = Get-JsonValue -Obj $f -Key "regex"
        }
        $j_path = Get-JsonValue -Obj $f -Key "path"
        $j_value = $proj.SelectSingleNode($j_path).InnerText
        if(($null -ne $j_regex) -and !($j_value -match $j_regex)){
            # 不符合则报错
            Write-Host "[Pre Build] [ERROR] | $item : $j_value <- Not Match Regex: $j_regex" -ForegroundColor red
            exit 1
        }
        $res_json[$item] = $j_value
    }
}

# Load Requires
$requires_value = @()
$nodes = $proj.SelectNodes("//Project/ItemGroup/PackageReference[@Include]")
foreach($node in $nodes){
    $require_name = $node.Attributes["Include"].Value
    if($require_name.StartsWith($prefix)){
        $requires_value += ($require_name + ">=" + $node.Attributes["Version"].Value)
    }
}
$res_json["requires"] = $requires_value
$res_json
# $meta = $proj.SelectSingleNode("//Project/MetaData")

# $proj.SelectNodes("//Project/ItemGroup[0]")
# if ($meta) {
#     foreach($item in $meta.ChildNodes){
#         if($meta_d.psobject.properties.match($item.Name).Count){
#             # 如果存在定义
#             $i_d =$meta_d.psobject.properties.Item($item.Name).Value
#             if($i_d.psobject.properties.match("regex").Count){
#                 # 如果存在正则表达式
#                 $regex = $i_d.psobject.properties.Item("regex").Value
#                 $value = $item.InnerText
#                 if(!($value -match $regex)){
#                     # 不符合则报错
#                     Write-Host "[Pre Build] [ERROR] | $($item.Name): $value <- Not Match Regex: $regex" -ForegroundColor red
#                     exit 1
#                 }
#             }
#         }
#     }
# }
# else{
#     Write-Host "[Pre Build] [ERROR] | MetaData Not Found" -ForegroundColor red
#     exit 1
# }