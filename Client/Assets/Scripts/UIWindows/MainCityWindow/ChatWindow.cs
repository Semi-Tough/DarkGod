/****************************************************
    文件：ChatWindow
    作者：wzq
    邮箱：1693416984@qq.com
    日期：2022年03月14日 星期一 11:53
    功能：聊天界面
*****************************************************/

using System.Collections.Generic;
using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class ChatWindow : WindowRoot
{
    public Transform btnChatGroup;
    public InputField inputChat;
    public ScrollRect scrollRect;

    public Text txtChat;
    private Image _imgFriend;
    private Image _imgGuild;
    private Image _imgWord;


    private bool _canSend = true;
    private ChatType _currentType;

    private readonly List<string> _chatList = new List<string>();


    protected override void InitWindow()
    {
        base.InitWindow();
        RegisterClickEvents();
        _currentType = ChatType.Word;
        ClickBtnChat(_currentType);
    }


    private void RegisterClickEvents()
    {
        for (int i = 0; i < btnChatGroup.childCount; i++)
        {
            Image image = btnChatGroup.GetChild(i).GetComponent<Image>();
            switch (i)
            {
                case 0:
                    _imgWord = image;
                    break;
                case 1:
                    _imgGuild = image;
                    break;
                case 2:
                    _imgFriend = image;
                    break;
            }

            OnClick(image.gameObject, obj =>
            {
                audioService.PlayUiAudio(Constants.UiClickBtn);
                _currentType = (ChatType) obj;
                ClickBtnChat(_currentType);
            }, i);
        }
    }

    private void ClickBtnChat(ChatType chatType)
    {
        switch (chatType)
        {
            case ChatType.Word:
            {
                string chatMessage = "";
                foreach (string chat in _chatList) chatMessage += chat + "\n";

                SetText(txtChat, chatMessage);

                inputChat.ActivateInputField();

                //在 UGUI 中在 GameObject 上面挂载 Content Size Fitter 时，会导致 RectTransform 的更新延迟一帧才能更新。
                //这应该是由于Content Size Fitter组件在每一帧的最后才去计算真正的高度等数值。
                //两种解决方案,方案一更消耗性能

                /*解决方案一  强制刷新页面 */
                // Canvas.ForceUpdateCanvases();
                // scrollRect.verticalNormalizedPosition = 0f;
                // Canvas.ForceUpdateCanvases();

                /*解决方案二   延迟一帧执行 */
                // StartCoroutine(UpdateContentHeight());
                // private IEnumerator UpdateContentHeight()
                // {
                //     yield return null;
                //     scrollRect.verticalNormalizedPosition = 0f;
                // }
                timerService.AddFrameTask(() => { scrollRect.verticalNormalizedPosition = 0f; }, 1, 1);

                SetSprite(_imgWord, PathDefine.ChatSelectedBtnImg);
                SetSprite(_imgGuild, PathDefine.ChatCommonBtnImg);
                SetSprite(_imgFriend, PathDefine.ChatCommonBtnImg);
            }
                break;
            case ChatType.Guild:
            {
                SetText(txtChat, "尚未加入公会...");

                SetSprite(_imgWord, PathDefine.ChatCommonBtnImg);
                SetSprite(_imgGuild, PathDefine.ChatSelectedBtnImg);
                SetSprite(_imgFriend, PathDefine.ChatCommonBtnImg);
                break;
            }
            case ChatType.Friend:
            {
                SetText(txtChat, "暂无好友信息...");

                SetSprite(_imgWord, PathDefine.ChatCommonBtnImg);
                SetSprite(_imgGuild, PathDefine.ChatCommonBtnImg);
                SetSprite(_imgFriend, PathDefine.ChatSelectedBtnImg);
            }
                break;
        }
    }


    public void AddMessage(string playerName, string chat)
    {
        _chatList.Add(Constants.Color(playerName + "：", TextColor.Blue) + chat);
        if (!GetWindowState()) return;
        ClickBtnChat(_currentType);
    }


    private enum ChatType
    {
        Word,
        Guild,
        Friend
    }

    #region 点击事件

    public void ClickSendBtn()
    {
        audioService.PlayUiAudio(Constants.UiClickBtn);
        if (!_canSend)
        {
            GameRoot.AddTips("消息发送过于频繁请稍后重试...");
            return;
        }

        if (!string.IsNullOrWhiteSpace(inputChat.text))
        {
            if (inputChat.text!.Length > 20)
            {
                GameRoot.AddTips("发送聊天消息不能超过20个字符...");
            }
            else
            {
                //发送网络消息到服务器
                GameMessage message = new GameMessage
                {
                    cmd = (int) Cmd.SendChat,
                    SendChat = new SendChat
                    {
                        Chat = inputChat.text
                    }
                };
                netService.SendMessage(message);
                inputChat.text = "";
                _canSend = false;
                timerService.AddTimeTask((() => { _canSend = true; }), 1, 1, PeTimeUnit.Second);
            }
        }
        else
        {
            GameRoot.AddTips("请输入聊天信息...");
        }
    }

    public void ClickCloseBtn()
    {
        audioService.PlayUiAudio(Constants.UiClickBtn);
        SetWindowState(false);
    }

    #endregion
}