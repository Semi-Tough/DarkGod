/****************************************************
	文件：LoginSystem.cs
	作者：wzq
	邮箱: 1693416984@qq.com
	日期：2022/02/27 13:09   	
	功能：登陆系统
*****************************************************/

using PEProtocol;

namespace Sever
{
    internal class LoginSystem : SystemRoot
    {
        private static LoginSystem _instance;
        public static LoginSystem Instance => _instance ?? (_instance = new LoginSystem());

        public override void Init()
        {
            base.Init();
            PeCommon.Log("LoginSystem Init Done.");
        }

        public void ResponseLogin(MessagePack messagePack)
        {
            RequestLogin data = messagePack.Message.RequestLogin;
            GameMessage message = new GameMessage
            {
                cmd = (int) Cmd.ResponseLogin
            };

            //当前账户是否已经上线
            if (CacheService.IsAccountOnLine(data.Account))
            {
                //已上线:返回错误消息
                message.err = (int) ErrorCode.AccountIsOnline;
            }
            else
            {
                //未上线:登陆账号
                //检测账户是否有存在
                //不存在账户:创建默认的账户密码
                PlayerData playerData = CacheService.GetPlayerDataByAccount(data.Account, data.Password);
                if (playerData == null)
                {
                    //存在账户,密码错误
                    message.err = (int) ErrorCode.WrongPassword;
                }
                else
                {
                    //存在账户,密码正确,拉取玩家数据

                    //计算离线体力增长
                    int power = playerData.Power;
                    int maxPower = PeCommon.GetPowerLimit(playerData.Level);

                    if (power < maxPower)
                    {
                        long nowTime = TimerService.GetNowTime();
                        long timeInterval = nowTime - playerData.Time;
                        int addPower = (int) (timeInterval / (PeCommon.PowerAddCount * 1000 * 60)) *
                                       PeCommon.PowerAddCount;
                        if (addPower > 0)
                        {
                            playerData.Power += addPower;
                            if (playerData.Power > maxPower) playerData.Power = maxPower;

                            if (!CacheService.UpDataPlayerDataToDataBase(playerData.Id, playerData))
                            {
                                PeCommon.Log("Update Player Power Error", PeLogType.Error);
                            }
                        }
                    }


                    message.ResponseLogin = new ResponseLogin
                    {
                        PlayerData = playerData
                    };
                    CacheService.AccountOnline(data.Account, messagePack.Session, playerData);
                }
            }

            //回应客户端
            messagePack.Session.SendMsg(message);
        }

        public void ResponseRename(MessagePack messagePack)
        {
            RequestRename rename = messagePack.Message.RequestRename;
            GameMessage message = new GameMessage
            {
                cmd = (int) Cmd.ResponseRename
            };

            if (!CacheService.IsNameExist(rename.Name))
            {
                //更新缓存
                PlayerData playerData = CacheService.GetPlayerDataBySession(messagePack.Session);
                playerData.Name = rename.Name;
                //数据库更新
                if (CacheService.UpDataPlayerDataToDataBase(playerData.Id, playerData))
                    message.ResponseRename = new ResponseRename
                    {
                        Name = rename.Name
                    };
                else
                {
                    message.err = (int) ErrorCode.UpDataDataBaseError;
                }
            }
            else
            {
                message.err = (int) ErrorCode.NameIsExist;
            }

            messagePack.Session.SendMsg(message);
        }

        public void ClearOfflineData(SeverSession session)
        {
            //写入下线时间
            PlayerData playerData = CacheService.GetPlayerDataBySession(session);
            if (playerData == null) return;
            playerData.Time = TimerService.Instance.GetNowTime();
            if (!CacheService.UpDataPlayerDataToDataBase(playerData.Id, playerData))
                PeCommon.Log("Update Offline Time Error", PeLogType.Error);

            CacheService.Instance.AccountOffline(session);
        }
    }
}