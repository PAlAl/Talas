$(document).ready(function () {
    $("#select").bind('click', updateDates);
    $("#select").click();  
    $('#overview').css({ display: 'none' }); 
    $("#graph").click();
    $("#overview").show("slow");  
});

function updateDates() {
    var dateStart = $('[name = dateStart]').val();
    if (dateStart != "") dateStart = "&dateStart=" + dateStart;
    var dateFinish = $('[name = dateFinish]').val();
    if (dateFinish != "") dateFinish = "&dateFinish=" + dateFinish;
    $("#donwload").attr("href", "/Engine/Download/?id="+$('[name = idEngine]').val() + dateStart + dateFinish);
}