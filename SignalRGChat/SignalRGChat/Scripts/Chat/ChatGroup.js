$(function ()
{
    //First time hide Group text box
    $('#txtGroupMessage').hide();                    // Text box for group hide
    $('#txtMessage').show();                         //public Text box show 

    $.connection.hub.start().done(function ()        //connection Start
    {
        $("#Connected").html("Connected");
    });

    //=========== Message To EveryOne Public ================
    var con = $.connection.myHub;
    $('#Send').click(function ()
    {
        var user = $('#txtUser').val();
        var msz = $('#txtMessage').val();

        con.server.sendMassege(user, msz);           //methodName camelCase 
        $('#txtMessage').val('');
    })


    $('#txtMessage').keypress(function (e)          //After type Enter key
    {

        if (e.which == 13)
        {
            var user = $('#txtUser').val();
            var msz = $('#txtMessage').val();

            con.server.sendMassege(user, msz);
            $('#txtMessage').val('');
        }

    })

    con.client.SendToAll = function (u, m)
    {
        $('#result').prepend('<b>' + u + '</b>:   ' + m + '<br/>');


    }

    //============= display old message Public ====================
    con.client.OldMessage = function (u, m, t)
    {
        $('#result').prepend('<b>' + u + '</b>: ' + m + '    <b style="color: rgba(128, 128, 128, .3)">' + t + '</b> </br>');
    }

    //======================OnLine User=============================*
    con.client.OnlineUser = function (n)
    {
        $('#OnlineUser').append('<li>' + n + '</li>');
    }
    con.client.OnlineDisconnectedUser = function (n)
    {
        var optionTexts = [];
        $('ul li').each(function () { optionTexts.push($(this).text()) });

        for (var i = 0; i < optionTexts.length; i++)
        {
            var delUser = n;
            var curUser = optionTexts[i];
            if (delUser == curUser)
            {
                $('li').filter(function () { return $.text([this]) === curUser; }).remove();
            }
        }
    }
    //=================================================================*

    //============= New User Connect Message + Time ===========
    con.client.NewUserConnected = function (User)
    {
        $('#UserConnect').prepend(User + '<br/>');
    }


    //================= DropDown Menu ====================
    $("#GroupNameDropDown").change(function ()
    {
        var GroupName = $('#txtGroupName').val() == '' ? $("#GroupNameDropDown").val() : $('#txtGroupName').val();
        con.server.join(GroupName).done(function ()
        {
            $("#Connected").html("You are in <span style='color: red'>" + GroupName + "</span> Group");
        })

        $('#resultGroup').empty();                         //Clear Previous Message
        $('#resultGroup').show();
        $('#txtGroupMessage').show();                       // Text box for group show
        $('#txtMessage').hide();                            //public Text box hide
        con.server.loadGroupMessage(GroupName);            //Load old Group message

        $('#result').hide();
    })

    //============== Create or Join Group ===============
    $('#btnCreateGroup').click(function ()                 //Same function as dropdown 
    {
        var GroupName = $('#txtGroupName').val() == '' ? $("#GroupNameDropDown").val() : $('#txtGroupName').val();

        if (GroupName != null)
        {
            con.server.join(GroupName).done(function ()
            {
                $('#txtGroupName').val('');
                $("#Connected").html("You are in <span style='color: red'>" + GroupName + "</span> Group");
                $('#resultGroup').show();
                $('#result').hide();
                $('#txtGroupMessage').show();                   // Text box for group show
                $('#txtMessage').hide();                         //public Text box hide
            })
        } else
        {
            $("#Connected").html("Which Group do you want to create<span style='color: red'>???</span> ");
        }
        
    })


    //============== Leave Group ==============
    $('#btnLeaveGroup').click(function ()
    {
        var GroupName = $('#txtGroupName').val() == '' ? $("#GroupNameDropDown").val() : $('#txtGroupName').val();

        con.server.leave(GroupName).done(function ()
        {
            if (GroupName != null)
            {
                $("#Connected").html("Please Join a Group first");
                $('#resultGroup').hide();                   //result div for group
                $('#result').show();                        //public Result
                $('#txtGroupMessage').hide();               // Text box for group hide
                $('#txtMessage').show();                    //public Text box show
            }
        })
    })


    //============== Send Group Message to a group ===============
    $('#SendSendToGoup').click(function ()
    {
        var GroupName = $('#txtGroupName').val() == '' ? $("#GroupNameDropDown").val() : $('#txtGroupName').val();
        var user = $('#txtGroupMessage').val();
        var msz = $('#txtMessage').val();

        con.server.sendGroupMassege(GroupName, user, msz);            //send message
        $('#resultGroup').show();
        $('#result').hide();
    })

    $('#txtGroupMessage').keypress(function (e)                          //Enter Keyboard workable
    {
        if (e.which == 13)
        {
            var GroupName = $('#txtGroupName').val() == '' ? $("#GroupNameDropDown").val() : $('#txtGroupName').val();
            var user = $('#txtUser').val();
            var msz = $('#txtGroupMessage').val();

            $('#resultGroup').show();
            $('#result').hide();

            con.server.sendGroupMassege(GroupName, user, msz);
            $('#txtGroupMessage').val('');
        }
    })

    con.client.SendToAllInGroup = function (u, msz)                 //received message
    {

        $('#resultGroup').prepend('<b>' + u + '</b>:  ' + msz + '<br/>');
    }

    //=================== Load Old GroupMessage =====================
    con.client.GroupOldMessage = function (uName, msz, time)
    {
        $('#resultGroup').prepend('<b>' + uName + '</b>:   ' + msz + '   <b style="color: rgba(128, 128, 128, .3)">' + time + '</b> <br/>');
    }
})


















// Created group show in Dropdown
// Enter key prass group and open
