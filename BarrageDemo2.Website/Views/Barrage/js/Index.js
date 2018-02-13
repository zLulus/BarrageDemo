$(function () {
    var clickScreen = $("#click_screen");
    var screen = $(".screen");
    var userNameBox = $(".s_userName");
    var commentBox = $(".s_txt");
    var bulletArea = $(".s_show");
    var commentButton = $(".s_btn");
    var connection = null;

    // 点击展开弹幕
    clickScreen.click(function () {
        // 创建连接
        connection = $.connection("/barrageConnection");
        connection.start().done(function () {
            screen.toggle(600);
        });
        connection.received(function (data) {
            var bulletObj = JSON.parse(data);
            // 接收服务器推送过来的弹幕消息
            addBullet(bulletObj.userName, bulletObj.comment);
        });
        return false;
    });

    // 点击关闭弹幕
    $(".s_close").click(function () {
        if (connection != null) {
            // 关闭服务器连接
            connection.stop(true, true);
            connection = null;
        }
        screen.toggle(600);
        return false;
    });

    // 发表评论
    commentButton.click(function () {
        var userName = userNameBox.val();
        var comment = commentBox.val();
        if (userName == "" && comment == "") {
            alert("请输入用户名和评论");
            return false;
        }
        addBullet(userName, comment);
        connection.send({ userName: userName, comment: comment });
    });

    // 按回车发表评论
    commentBox.keydown(function () {
        var code = window.event.keyCode;
        if (code == 13)//回车键按下时，输出到弹幕
        {
            commentButton.click();
        }
    });

    function addBullet(userName, comment) {
        bulletArea.append("<div>" + userName + ":" + comment + "</div>");
        refreshBullets();
    }

    // 下面部分代码参考自http://www.zyfun.cn/2015/04/81/
    // 刷新显示弹幕
    function refreshBullets() {
        var _top = 0;

        bulletArea.find("div").show().each(function () {
            var bullet = $(this);
            var _left = $(window).width() - bullet.width();
            var _height = $(window).height();

            _top = _top + 80;

            if (_top > _height - 100) {
                _top = 80;
            }

            var time = 10000;
            if (bullet.index() % 2 == 0) {
                time = 20000;
            }
            //设定文字的初始化位置
            bullet.css({ left: _left, top: _top, color: getRandomColor() });
            bullet.animate({ left: "-" + _left + "px" }, time, function () {
                bullet.remove();
            });
        });
    }

    //随机获取颜色值
    function getRandomColor() {
        return '#' + (function (h) {
            return new Array(7 - h.length).join("0") + h
        })((Math.random() * 0x1000000 << 0).toString(16))
    }
});

