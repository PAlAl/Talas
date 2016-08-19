function show() {
    var classes = $("input:hidden").map(function (indx, element) {
        return $(element).val();
        });
    var arr = classes.get();
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

$(document).ready(function () {
    show();
    setInterval('show()', 2000);
});

