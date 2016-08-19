var a = [];
var b = [];
var dateT;
function getdata() {
    a = [];
    b = [];
    dateT = document.getElementById("calendar").value;
    if (dateT != "")
        dateT += "-01";

    $.ajax({
        url: 'view.php',
        type: "GET",
        data: { requestGraph: dateT },/*"2016-03-15"*/
        success: function (response) {
            if (dateT != "")
                response = response.reverse();
            while (response.length > 0) {
                var element = response.pop();
                a.push(element["value"]);
                b.push(element["date"]);
            }	
            showGraph();
        },
        dataType: "json"
    });
};


$(document).ready(function () {
    $('#overview').css({ display: 'none' });
    getdata();
});

function showGraph() {
    setTimeout(showHide, 1000)
    setTimeout(drwGrph, 1500)
}

function showHide() {
    if (a.length > 1) {
        $("#overview").show("slow");
    }
    else {
        $("#overview").hide("slow");
    }
}

Date.prototype.addDays = function (d) {
    this.setDate(this.getDate() + d);
    return this;
};

function drwGrph() {
    if (a.length > 1) {
        var dayT, monthT;
        if (dateT != "") {
            dayT = 1;
            monthT = dateT.substring(6, 7);
        }
        else {
            var d = (new Date()).addDays(-30); // Дата на 10 дней раньше текущей
            dayT = d.getDate();
            monthT = d.getMonth() + 1;
        }

        var chart1 = new Highcharts.Chart({
            chart: {
                renderTo: 'overview',
            },
            title: {
                text: 'Data'
            },
            xAxis: {
                categories: b
            },
            yAxis: {
                title: {
                    text: 'Insulation Resistance,kOhm'
                },
                min: 0,
                minorGridLineWidth: 0,
                gridLineWidth: 0,
                alternateGridColor: null,
                plotBands: [{ // Light air
                    from: 0,
                    to: 500,
                    color: '#F36666'
                }, { // Fresh breeze
                    from: 500,
                    to: 1100,
                    color: '#f1a165'
                }, { // High wind
                    from: 1100,
                    to: 10000,
                    color: '#66F380'
                }
                ]
            },
            tooltip: {
                formatter: function () {
                    return '<b>' + this.series.name + '</b><br/>' +
                    this.x + ': ' + this.y + ' kOhm';
                }
            },
            series: [{
                name: 'Insulation Resistance',
                data: a,
                shadow: true,

            }]
        });
    }
    else {
        $("#overview").hide("slow");
    }
}
