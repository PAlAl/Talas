function dash() {
    $("a[data-ajax-url*='/Home/Engine']").click();
}

$(document).ready(function () {
    dash();
    setTimeout('showEnginesDispatching()', 2000);
    setTimeout('showMessageDispatching()', 2000);
    goShow();     
});

function goShow()
{
    setInterval('showEnginesDispatching()', 30000);
    setInterval('showMessageDispatching()', 30000);
}

function clearLink()
{
    $(".callOnChanheDatePicker a").each(function () {
        var url = $(this).attr('data-ajax-url');
        if (url != null && url.lastIndexOf("dateStart") != -1) {
            url = url.slice(0, -43);
            $(this).attr('data-ajax-url', url);
        }
    });
}

function datePickerChange() {
    var dateStart = $("[name=dateStart]").val();//datepicker("getDate");
    var dateFinish = $("[name=dateFinish]").val()//datepicker("getDate");
    if (dateStart != "" && dateFinish != "") {
        $(".callOnChanheDatePicker a").each(function () {
            var url = $(this).attr('data-ajax-url');
            if (url != null)
            {
                if (url.lastIndexOf("dateStart") != -1) clearLink();
                url = url + "&dateStart=" + dateStart + "&dateFinish=" + dateFinish;
                $(this).attr('data-ajax-url', url);
            }
            });
    }
    else
    {
        clearLink();
    }
}

function onActiveDownButtons(event) {
    var elemLi = event.currentTarget;
    $("li.buttonNav.active").each(function (i,elem) {
        $(elem).removeClass("active");
    });
    $(elemLi).addClass("active");    
}
function onSuccess(mode) {
    if ($(".datepick").hasClass("hidden"))
        showDatePickForm();

    if (mode == 3)
        showInsulationResistBtns();
    else
        hideInsulationResistBtns();

    if (mode == 2) {
        showWorkInfo();
        $('#work').text($('[name=work]').val());
        $('#notWork').text($('[name=notWork]').val());
    }
    else      
        hideWorkInfo();
    

    var url = $('.datepick').attr('data-ajax-url');
    if (url[url.length-1] != '=')
        url =  url.slice(0, -1);
    url = url + mode;
    $('.datepick').attr('data-ajax-url', url);

    if ($('[name=dateStart]').val() != "" || $('[name=dateFinish]').val() != "") 
    {
        update();
    }
}

function showInsulationResistBtns()
{
    $('#InsilationResistButtons').removeClass("hidden");
    $('[name=viewType],[value=table]').prop("checked", true);
    $('[name=viewType],[value=graph]').prop("checked", false);
    $('#labelTable').addClass("active");
    $('#labelGraph').removeClass("active");
}

function update()
{
    $('[value=Show]').click();
}

function hideInsulationResistBtns() {
    if (!$("#InsilationResistButtons").hasClass("hidden")) {
        $('#InsilationResistButtons').addClass("hidden");
    }
}

function showDatePickForm() {
        $('.datepick').removeClass("hidden");
}

function hideDatePickForm() {
        $('.datepick').addClass("hidden");
}

function hideWorkInfo() {
    if (!$('#workInfo').hasClass("hidden"))
        $('#workInfo').addClass("hidden");
    $('a:contains("GetMessages")').click();
}

function showWorkInfo() {
    $('#workInfo').removeClass("hidden");
}

function showEnginesDispatching() {
    var classes = $("[name=id]").filter(':input').map(function (indx, element) {
        return $(element).val();
    });
    var arr = classes.get();
    if (arr.length > 0) {      
        $.ajax({
            url: 'Home/JsonGetInfo',
            type: 'POST',
            dataType: 'json',
            data: { Engines: arr },
            success: function (response) {
                response = response.reverse();
                while (response.length > 0) {
                    var element = response.pop();
                    if (element != null) {
                        var id = element.EngineId;
                        $('[name = Date№' + id + ']').text(element.DateString);
                        if (element.Engine != null && element.Engine.IsClamp)
                            $('[name = Value№' + id + ']').text(element.Value / 1000000);
                        else {
                            if (element.Engine != null && element.Engine.IsTes)
                                $('[name = Value№' + id + ']').text(element.Value == null ? 0 : (element.Value > 1000 ? "High" : element.Value));
                            else
                                $('[name = Value№' + id + ']').text(element.Value == null ? 0 : (element.Value > 20000 ? "High" : element.Value));
                        }
                        $('[name = Work№' + id + ']').text(element.Work == null ? 0 : element.Work ? "ON" : "OFF");
                        $('[name = Status№' + id + ']').text(element.Status_M == null ? 0 : element.Status_M ? "ON" : "OFF");
                    }
                }
            }
        });
    }
}

function showMessageDispatching() {
    if ($("#isGeneral").text() != 1)
        $.ajax({
            url: 'Home/JsonGetNumberNewMessages',
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                if (response > 99 && $('.labelAlert').css('width') == '35px')
                    $('.labelAlert').css('width', 45);
                if (response < 100 && $('.labelAlert').css('width') == '45px')
                    $('.labelAlert').css('width', 35);
                if (response != $('.labelAlert').text())
                {
                    $('.labelAlert').text(response);
                    $('a:contains("GetMessages")').click();
                }              
                }
            
        });
}

