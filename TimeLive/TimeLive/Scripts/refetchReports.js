
$(document).ready(function () {

    $.ajax({

        url: "/time/getreports/",
        datatype: "json",
        data: {},
        success: function (data) {
            var list = data.reports;

            //$("#reportList").html(""); //Så rapporter ej stackas

            for (i = 0; i < list.length; i++) {
                //alert(data)

                var listOfShit = list[i];

                var shitToWrite = $("#reportList").html($("#reportList").html()
                    //+ "<div class='row'>"
                    //        + "<div>" + "invoicedtime: " + listOfShit.title + "</div>"
                    //+ "</div>"
                    + "<div class='col col-dropdown'">
                       + "<input type='hidden' class='companyId' name='pCompanyId' value= />"
                       + "<select class='form-control nopadding companyDropDown'></select>"
                    + "</div>"
                    );
            }
        },//Success

        error: function (jqXHR, statusText, errorThrown) {
            $('#reportList').html('Ops something happened!: <br>'
                + statusText + " " + errorThrown + " " + jqXHR);
        }//Error
    });//ajax
});//Document.Ready