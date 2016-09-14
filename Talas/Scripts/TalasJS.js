function dash() {
    $("a[data-ajax-url*='/Home/Engine']").click();
}

$(document).ready(function () {
    dash();
    setTimeout(goShow(), 2000);
});

function goShow()
{
    setInterval('show()', 2000);
}

function onSuccess(mode) {
    $('[name=dateStart]').val("");
    $('[name=dateFinish]').val("");
    ShowHideDatePickForm(0);
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

function updateInsilationResist()
{
    $('[value=Show]').click();
   // var getvalue = $("[name=viewType],[value=table]").attr('cheked');
    //alert(getvalue);
}

function hideInsulationResistBtns() {
    $('#InsilationResistButtons').addClass("hidden");
}

function ShowHideDatePickForm(mode) {
    if (mode)
        $('.datepick').addClass("hidden");
    else
        $('.datepick').removeClass("hidden");
}

function show() {
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
                    $('[name = Work№' + id + ']').text(element.Work == null ? 0 : element.Work);
                    $('[name = Status№' + id + ']').text(element.Status == null ? 0 : element.Status);
                    $('[name = Test№' + id + ']').text(element.Test == null ? 0 : element.Test);
                }
            }
        });
    }
}