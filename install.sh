aiur() { arg="$( cut -d ' ' -f 2- <<< "$@" )" && curl -sL https://gitlab.aiursoft.com/aiursoft/aiurscript/-/raw/master/$1.sh | sudo bash -s $arg; }
warp_path="/opt/apps/Warp"

install_warp()
{
    port=$(aiur network/get_port) && echo "Using internal port: $port"
    aiur network/enable_bbr
    aiur install/caddy
    aiur install/dotnet
    aiur git/clone_to https://gitlab.aiursoft.com/aiursoft/Warp ./Warp
    aiur dotnet/publish $warp_path ./Warp/Warp.csproj
    aiur services/register_aspnet_service "warp" $port $warp_path "Aiursoft.Warp"
    aiur caddy/add_proxy $1 $port

    echo "Successfully installed Warp as a service in your machine! Please open https://$1 to try it now!"
    rm ./Warp -rf
}

# Example: install_tracer http://warp.local
install_warp
