function dash() {
    $("a[data-ajax-url*='/Home/Engine']").click();
}

$(document).ready(function () {
    dash();
    setTimeout(goShow(), 2000);
});

function goShow()
{
    setInterval('showEnginesDispatching()', 2000);
    setInterval('showMessageDispatching()', 5000);
}

function datePickerChange() {
    var dateStart = $("[name=dateStart]").val();
    var dateFinish = $("[name=dateFinish]").val();    
    if (dateStart != "" && dateFinish != "") {
        $(".callOnChanheDatePicker a").each(function () {
                var url = $(this).attr('data-ajax-url');              
                if (url != null && url.lastIndexOf("dateStart") == -1 )                    
                    {
                        url = url + "&dateStart=" + dateStart + "&dateFinish=" + dateFinish;
                        $(this).attr('data-ajax-url', url);
                    }
            });
    }
    else
    {
        $(".callOnChanheDatePicker a").each(function () {
                var url = $(this).attr('data-ajax-url');
                if (url != null && url.lastIndexOf("dateStart") != -1) {
                    url = url.slice(0, -43);
                    $(this).attr('data-ajax-url', url);
                }
            });

    }
}

function onSuccess(mode) {
    if ($(".datepick").hasClass("hidden"))
        showDatePickForm();

    if (mode == 3)
        showInsulationResistBtns();
    else
        hideInsulationResistBtns();

    var url = $('.datepick').attr('data-ajax-url');
    if (url[url.length-1] != '=')
        url =  url.slice(0, -1);
    url = url + mode;
    $('.datepick').attr('data-ajax-url', url);
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
                    var id = element.EngineId;
                    $('[name = Date№' + id + ']').text(element.Date == null ? 0 : new Date(parseInt(element.Date.substr(6))).toLocaleString());
                    $('[name = Value№' + id + ']').text(element.Value == null ? 0 : element.Value);
                    $('[name = Work№' + id + ']').text(element.Work == null ? 0 : element.Work?"ON":"OFF");
                    $('[name = Status№' + id + ']').text(element.Status == null ? 0 : element.Status ? "ON" : "OFF");
                    //$('[name = Test№' + id + ']').text(element.Test == null ? 0 : element.Test);
                }
            }
        });
    }
}

function showMessageDispatching() {    
        $.ajax({
            url: 'Home/JsonGetNumberNewMessages',
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                $('.labelAlert').text(response);
                }
            
        });
}

