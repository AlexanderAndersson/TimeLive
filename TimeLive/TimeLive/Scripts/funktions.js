/// <reference path="moment.js" />
/// <reference path="moment.js" />
$(document).ready(function (date) {

    if (sessionStorage.View === undefined) {
        sessionStorage.View = "agendaWeek";
    };

    //Customizations for week calendar
    $('#week').fullCalendar({
        theme: true,
        header: false,
        editable: true,
        defaultView: sessionStorage.View,
        defaultDate: sessionStorage.Date,
        weekNumberCalculation: "ISO",
        editable: false,
        allDaySlot: false,
        selectable: true,
        lazyFetching: true,
        firstDay: 1,
        fixedWeekCount: false,
        weekNumbers: true,
        weekends: false,
        height: "auto",
        columnFormat: 'ddd',
        displayEventTime: false,
        timeFormat: 'h:mm',
        events: "/Time/GetWeekEvents/",
        slotDuration: "00:60:00",
        eventBorderColor: "none",
        businessHours: true,
        businessHours: {
            dow: [1, 2, 3, 4, 5], // Monday, Tuesday, Wednesday....
            start: '01:00',
            end: '09:00',
        },//businessHours
        slotLabelFormat: 'H(:mm)',
        minTime: "01:00:00",
        maxTime: '09:00:00',
        eventMouseover: function (data, event, view) {
            tooltip = '<div class="tooltiptopicevent" style="width:auto; height:auto; font-weight:bold; background:#25849a; color:white; border:1px solid black; position:absolute; z-index:10001; padding:5px 5px 5px 5px; line-height:200%; ">' + 'Used: ' + data.title + '</br>' + 'Invoiced: ' + data.invoiced + '</br>' + 'Company: ' + data.description + '</div>';

            $("body").append(tooltip);
            $(this).mouseover(function (e) {
                $(this).css('z-index', 10000);
                $('.tooltiptopicevent').fadeIn('500');
                $('.tooltiptopicevent').fadeTo('10', 1.9);
            }).mousemove(function (e) {
                $('.tooltiptopicevent').css('top', e.pageY + 10);
                $('.tooltiptopicevent').css('left', e.pageX + -145);
            });
        },
        eventMouseout: function (data, event, view) {
            $(this).css('z-index', 8);
            $('.tooltiptopicevent').remove();
        },//Mouseover
    });//Week

    //Customizations for month calendar
    $('#month').fullCalendar({
        theme: true,
        header: {
            editable: false,
            left: 'prev',
            center: 'title',
            right: 'next'
        },//header
        lazyFetching: true,
        defaultView: 'month',
        defaultDate: sessionStorage.Date,
        weekNumberCalculation: 'ISO',
        events: "/Time/GetMonthEvents/",
        //viewRender was here and events after that
        editable: false,
        allDaySlot: true,
        selectable: true,
        firstDay: 1,
        fixedWeekCount: false,
        weekNumbers: true,
        weekends: false,
        height: "auto",
        navLinks: true,
        eventRender: function (event) {
            var eventStart = event.start.format("HH:mm");
            var eventEnd = event.end.format("HH:mm");
            if (eventEnd >= "09:00" || eventStart >= "09:00") { //if there is atleast 8 hours reported that day, the color is green   
                var dateEnd = event.end.format("YYYY-MM-DD");
                $('#month').find('.ui-widget-content[data-date=' + dateEnd + ']').css('background', '#128f12', "!important");
            }
            else { //if there is less than 8 hours reported that day, the color is red  
                var dateStart = event.start.format("YYYY-MM-DD");
                $('#month').find('.ui-widget-content[data-date=' + dateStart + ']').css('background', '#f23636', "!important");
            }
        },//EventRender
        dayRender: function (date, cell) {
            //if (date.format("YYYY-MM-DD") >= moment().a.format("YYYY-MM-DD")) { //if date is after today, the background will have no color
            //    var dateAfterToday = date.format("YYYY-MM-DD");
            //    $('#month').find('.fc-day-top[data-date=' + dateAfterToday + ']').css('background', 'none', "!important");
            //}
            if (date.format("YYYY-MM-DD") < moment().format("YYYY-MM-DD")) { //if there is no event on a day before today, the background will have a red color
                var dateBeforeToday = date.format("YYYY-MM-DD");
                $('#month').find('.ui-widget-content[data-date=' + dateBeforeToday + ']').css('background', '#f23636');
            }
        },//DayRender
        navLinkWeekClick: function (date) {
            $('#week').fullCalendar('gotoDate', date);
            $('#week').fullCalendar('changeView', 'agendaWeek');
            sessionStorage.View = "agendaWeek";
            sessionStorage.Date = date;
            $('#selectionFrom').val(date.format('YYYY-MM-DD'));
            $('#selectionTo').val(date.isoWeekday(5).format('YYYY-MM-DD'));
            $('#selectionsApply').click();
        },//navLinkWeekClick
        navLinkDayClick: function (date) {
            $('#week').fullCalendar('gotoDate', date);
            $('#week').fullCalendar('changeView', 'agenda');
            sessionStorage.View = "agenda";
            sessionStorage.Date = date;
            $('#selectionFrom').val(date.format('YYYY-MM-DD'));
            $('#selectionTo').val(date.format('YYYY-MM-DD'));
            $('#selectionsApply').click();
        },//navLinkDayClick 
        viewRender: function (view) { //Takes the first and last date of month, and stores it in Sessionstorage
            sessionStorage.firstDay = view.intervalStart.format("YYYY-MM-DD"); //First day of the month

            var newdate = new Date(view.start);
            newdate.setDate(newdate.getDate() + 7);
            var nd = new Date(newdate);
            sessionStorage.secondWeek = nd.toLocaleDateString();//The second week of the month

            var newdate = new Date(view.intervalEnd);
            newdate.setDate(newdate.getDate() - 1);
            var nd = new Date(newdate);
            sessionStorage.lastDay = nd.toLocaleDateString(); //Last day of the month
        },//ViewRender
    });//Month

    //Makes so that when you reload the page, the right image is showned on delays.
    $('.delayChk').each(function () {
        var checked = $(this).is(':checked');
        var image = $(this).siblings("img");

        if (checked == true) {
            image.attr('src', '/img/delay_true.png');
            $(this).addClass('check');
        }
        else {
            image.attr('src', '/img/delay_false.png');
            $(this).removeClass('check');
        }
    });

    //Shows all the reports for the whole month
    $('.fc-center').on('click', function () {
        sessionStorage.Date = sessionStorage.secondWeek;
        //$('#week').fullCalendar('changeView', 'agendaWeek');
        sessionStorage.View = "agendaWeek";
        $('#selectionFrom').val(sessionStorage.firstDay);
        $('#selectionTo').val(sessionStorage.lastDay);
        $('#selectionsApply').click();
    });

    //Script for active navbar
    if (typeof (Storage) !== "undefined") {
        if (sessionStorage.selectedNav) {
            $(".navImg").siblings().eq(sessionStorage.selectedNav).
            addClass("active");
        }
        else {
            $(".navImg").siblings().eq(2).addClass("active");
        }
    }
    $(".navImg").on("click", function () {
        $(this).siblings().removeClass("active");
        $(this).addClass("active");

        sessionStorage.selectedNav = $(".navImg").siblings().index(this);
    });

    //Clicking on a alert notification makes the page "focus" on that day
    $('#missingRpt').click(function () {
        var date = document.getElementById('missingReport').innerHTML;
        sessionStorage.Date = date;
        sessionStorage.View = "agenda";
        $('#selectionFrom').val(date);
        $('#selectionTo').val(date);
        $('#selectionsApply').click();
    });

    var modal = document.getElementById('myModal');

    //Get the <span> element that closes the modal
    var span = document.getElementsByClassName("close")[0];

    //When the user clicks on <span> (x), close the modal
    $(span).click(function () {
        modal.style.display = "none";
    });

    //When the user clicks anywhere outside of the modal, close it
    window.onclick = function (event) {
        if (event.target == modal) {
            modal.style.display = "none";
        }
    };

    //When clicking on error message the modal becomes visible
    $('#errorMsg').click(function () {
        modal.style.display = "block";
    });

    //$('#datesShowned').html(sessionStorage.From + " - " + sessionStorage.To)

});//document.ready

$('.newSubProject').change(function () {
    var subprojectid = this.value;
    var subProjectText = $(this).children(':selected').text();

    var row = $(this).parents('div');
    var buttons = row.find('.hideButton');
    var regdate = row.find('.newRegDate');
    var invoiced = row.find('.newInvoice');
    var used = row.find('.newUsed');
    var extComment = row.find('.newExtComment');
    var vacationFrom = row.find('.vacationFrom');
    var vacationTo = row.find('.vacationTo');


    if (subprojectid === "20071225 15:44:01:967"/*Semester*/ || subprojectid === "20100817 21:08:28:873" /*Föräldrarledigt*/
        || subprojectid === "20130130 08:07:17:807" /*Tjänstledigt*/) {
        regdate.addClass('hide');
        invoiced.addClass('hide');
        buttons.addClass('hide');
        vacationFrom.removeClass('hide');
        vacationTo.removeClass('hide');
        used.val(8);
        extComment.val(subProjectText);
    }
    else {
        regdate.removeClass('hide');
        invoiced.removeClass('hide');
        buttons.removeClass('hide');
        vacationFrom.addClass('hide');
        vacationTo.addClass('hide');
    }
});


//Changes picture when clicking on delay img
$('.delayChk').on('click', function () {
    var checked = $(this).is(':checked');
    var image = $(this).siblings("img");
    var input = $(this).siblings('.pDealyinvoice');
    if (checked == false) {
        image.attr('src', '/img/delay_false.png');
        input.val(0);
        $(this).removeClass('check');
    }
    else {
        image.attr('src', '/img/delay_true.png');
        input.val(1);
        $(this).addClass('check');
    }
});

//Changes the value of a new report when clicking on one of the 5 latest reports
$('#latest-container a').click(function () {
    var row = $(this);
    var companyId = row.find('input[name$=pCompanyId]').val();
    var projectId = row.find('input[name$=pProjectId]').val();
    var subProjectId = row.find('input[name$=pSubProjectId]').val();
    
    $('.newCompany').val(companyId).change();
    $('.newProject').val(projectId).change();
    $('.newSubProject').val(subProjectId).change();
});

//Copy button funtion
$('.copy').click(function () {
    var row = $(this);
    var companyId = row.find('input[name$=pCompanyId]').val();
    var projectId = row.find('input[name$=pProjectId]').val();
    var subProjectId = row.find('input[name$=pSubProjectId]').val();
    var invoiced = row.find('input[name$=pInvoicedTime]').val();
    var used = row.find('input[name$=pUsedTime]').val();
    var extComment = row.find('input[name$=pExternComment]').val();
    var IntComment = row.find('input[name$=pInternComment]').val();

    $('.newCompany').val(companyId).change();
    $('.newProject').val(projectId).change();
    $('.newSubProject').val(subProjectId).change();
    $('.newInvoice').val(invoiced).change();
    $('.newUsed').val(used).change();
    $('.newExtComment').val(extComment).change();
    $('.newIntComment').val(IntComment).change();
});


//Increase/decrease numerics with arrow keys
$('.amount').keydown(function (event) {
    var currentNumber = Number($(this).val());
    if (event.which === 38)//up
    {
        if (currentNumber % 1 == 0 || currentNumber % 1 == 0.5)
            $(this).val(Math.max(0, currentNumber + 0.50).toFixed(2)); //Plus 0.50
        else
            $(this).val(Math.max(0, Math.ceil(currentNumber * 2)) / 2) //Rounds up to nearest 0.50 number
    }       
    else if (event.which === 40)//down
    {
        if (currentNumber % 1 == 0 || currentNumber % 1 == 0.5)
            $(this).val(Math.max(0, currentNumber - 0.50).toFixed(2)); //Minus 0.50
        else
            $(this).val(Math.floor(currentNumber * 2) / 2); //Rounds down to nearest 0.50 number
    }
});

//Increase numerics with (plus button)
$('.btn-plus').on('click', function () {
    var currentNumber = $(this).parent().siblings('input').val();
    var field = $(this).parent().siblings('input');
    $(this).parent().siblings('input').val(Math.max(0, parseFloat($(this).parent().siblings('input').val()) + 0.50).toFixed(2))
    if (currentNumber > 7.50)
        $(field).css("background-color", "#febcbc"); //red
    else
        $(field).css("background-color", "#cbefb8"); //green
});

//Decrease numerics with (minus button)
$('.btn-minus').on('click', function () {
    var currentNumber = $(this).parent().siblings('input').val();
    var field = $(this).parent().siblings('input');
    $(this).parent().siblings('input').val(Math.max(0, parseFloat($(this).parent().siblings('input').val()) - 0.50).toFixed(2))
    if (currentNumber <= 8.50)
        $(field).css("background-color", "#cbefb8"); //green
});


//if invoice or used are more than 8, make backgound-color "red" otherwise "green"
$('.hours').keydown(function () {
    var currentNumber = Number($(this).val());
    var field = $(this);
    if (currentNumber > 8.00) {
        $(field).css("background-color", "#febcbc"); //red
    }
    else {
        $(field).css("background-color", "#cbefb8"); //green
    }
});


/* Date picker */
$('.date-picker').datepicker({
    dateFormat: "yy-mm-dd",
    showAnim: "slideDown",
    firstDay: 1,
    autoSize: true,
    beforeShowDay: $.datepicker.noWeekends
    //showOtherMonths: true,
    //selectOtherMonths: true,
    //showWeek: true,
});


//if comments are more or less than 60 characters. Make the background-color "red" or "green"
$('.comment').keyup(function () {

    var field = $(this);
    if (field.val().length > 0 && field.val().length <= 60) {
        $(field).css("background-color", "#cbefb8"); //green
    }
    else {
        $(field).css("background-color", "#febcbc"); //red
    }
});


//Show internal comment and hide siblings
$('.col > input').focus(function () {
    var timerow = $(this).parents('.timerow');
    timerow.find('.toggle').show(200);
    timerow.siblings().find('.toggle').hide(200);
});

$('#toggleMenu2').click(function () {
    $('#filter2').slideToggle("slow");
    //$('.sidebar').css("display", "block");
    //var div = $(".sidebar");
    //div.animate({ width: '280px' }, "slow");
    //div.animate({ height: '670px' }, "slow");
});

$('#toggleMenu').click(function () { //Toggle filter in navbar
    $('#filter').toggle("slide");
});


$('#toggleMenu').click(function () {
    $('.sidebarDistance').toggle("slide");
});

$('#toggleMenu').click(function () {
    $('.sidebarExpense').toggle("slide");
});


//$(document).click(function () {
//    if ($('.sidebar').css('display') == 'block') {
//        $('.sidebar').toggle("slide");
//    }
//});

$('#QlikView').click(function() {
    window.open("http://qv/qlikview/");
});

$('#quickToday').click(function () {
    $('#selectionFrom').val(moment().format('YYYY-MM-DD'));
    $('#selectionTo').val(moment().format('YYYY-MM-DD'));
    $('#selectionsApply').click();
});

$('#quickCWeek').click(function (date) {
    var startOfWeek = moment().isoWeekday(1);
    $('#selectionFrom').val(startOfWeek.format('YYYY-MM-DD'));
    $('#selectionTo').val(moment().isoWeekday(7).format('YYYY-MM-DD'));
    $('#selectionsApply').click();
});

$('#quickCMonth').click(function () {
    var startOfMonth = moment().startOf('month');
    $('#selectionFrom').val(startOfMonth.format('YYYY-MM-DD'));
    $('#selectionTo').val(moment().endOf('month').format('YYYY-MM-DD'));
    $('#selectionsApply').click();
});

$('#quickLMonth').click(function () {
    var previousMonth = moment().subtract(1, 'month');
    var startOfMonth = previousMonth.startOf('month');
    $('#selectionFrom').val(startOfMonth.format('YYYY-MM-DD'));
    var endOfMonth = previousMonth.endOf('month');
    $('#selectionTo').val(endOfMonth.format('YYYY-MM-DD'));
    $('#selectionsApply').click();
});

//$('#logo-time').click(function () {
//    var field = $(this);
//    $(field).removeclass("Inactive").addClass("Active");
//    alert()
//});
