/// <reference path="moment.js" />
/// <reference path="moment.js" />
$(document).ready(function (date) {

    if (localStorage.View === undefined) {
        localStorage.View = "agendaWeek";
    };

    //alert(localStorage.first);

    $('#week').fullCalendar({
        theme: true,
        header: false,
        editable: true,
        defaultView: localStorage.View,
        defaultDate: localStorage.Date,
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
        defaultDate: localStorage.Date,
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
                $('#month').find('.fc-day-top[data-date=' + dateEnd + ']').css('background', '#128f12', "!important");
            }
            else { //if there is less than 8 hours reported that day, the color is red  
                var dateStart = event.start.format("YYYY-MM-DD");
                $('#month').find('.fc-day-top[data-date=' + dateStart + ']').css('background', '#fe2d2d', "!important");
            }
        },//EventRender
        dayRender: function (date, cell) {
            //if (date.format("YYYY-MM-DD") >= moment().a.format("YYYY-MM-DD")) { //if date is after today, the background will have no color
            //    var dateAfterToday = date.format("YYYY-MM-DD");
            //    $('#month').find('.fc-day-top[data-date=' + dateAfterToday + ']').css('background', 'none', "!important");
            //}
            if (date < moment()) { //if there is no event on a day before today or today, the background will have a red color
                var dateBeforeToday = date.format("YYYY-MM-DD");
                $('#month').find('.fc-day-top[data-date=' + dateBeforeToday + ']').css('background', '#fe2d2d');
            }
        },//DayRender
        navLinkWeekClick: function (date) {
            $('#week').fullCalendar('gotoDate', date);
            $('#week').fullCalendar('changeView', 'agendaWeek');
            localStorage.View = "agendaWeek";
            localStorage.Date = date;
            $('#selectionFrom').val(date.format('YYYY-MM-DD'));
            $('#selectionTo').val(date.isoWeekday(7).format('YYYY-MM-DD'));
            $('#selectionsApply').click();
        },//navLinkWeekClick
        navLinkDayClick: function (date) {
            $('#week').fullCalendar('gotoDate', date);
            $('#week').fullCalendar('changeView', 'agenda');
            localStorage.View = "agenda";
            localStorage.Date = date;
            $('#selectionFrom').val(date.format('YYYY-MM-DD'));
            $('#selectionTo').val(date.format('YYYY-MM-DD'));
            $('#selectionsApply').click();
        },//navLinkDayClick 
        //viewRender with jQuert.ajax
    });//Month
});//document.ready

$('#latest-container a').click(function () {
    var row = $(this);
    var companyId = row.find('input[name$=pCompanyId]').val();
    var projectId = row.find('input[name$=pProjectId]').val();
    var subProjectId = row.find('input[name$=pSubProjectId]').val();
    
    $('.newCompany').val(companyId).change();
    $('.newProject').val(projectId).change();
    $('.newSubProject').val(subProjectId).change();
});

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

//Increase/decrease numerics with arrow keys
$('.amount-ints').keydown(function (event) {
    var currentNumber = Number($(this).val());
    if (event.which === 38)//up
    {
        if (currentNumber % 1 == 0)
            $(this).val(Math.max(0, currentNumber + 1).toFixed(0)); //Plus 1
        else
            $(this).val(Math.max(0, Math.ceil(currentNumber))) //Rounds up to nearest int
    }
    else if (event.which === 40)//down
    {
        if (currentNumber % 1 == 0)
            $(this).val(Math.max(0, currentNumber - 1).toFixed(0)); //Minus 1
        else
            $(this).val(Math.floor(currentNumber)); //Rounds down to nearest int
    }
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


//reimbuse checkbox function
$('.reimChk').change(function () {
    var checked = $(this).is(':checked');
    $(this).siblings('.pReimburse').val(checked ? 1 : 0);
});


$('.input_class_checkbox').each(function () {
    $(this).hide().after('<div class="class_checkbox" />');

});

//Makes so that when you reload the page, the right image is showned on delays.
$('.delayChk').each(function () {
    var checked = $(this).is(':checked');
    var image = $(this).siblings("img");
    if (checked == true) {
        image.attr('src', 'img/delay_true.png');
    }
    else {
        image.attr('src', 'img/delay_false.png');
    }
});


$('.delayChk').on('click', function () {
    var checked = $(this).is(':checked');
    var image = $(this).siblings("img");
    var input = $(this).siblings('.pDealyinvoice');
    $(this).toggleClass('checked').prev().prop('checked', $(this).is('.checked'))
    if (checked == true) {
        image.attr('src', 'img/delay_false.png');
       $(this).siblings('.pDealyinvoice').val(1);
        //input.val(1);
    }
    else {
        image.attr('src', 'img/delay_true.png');
        //$(this).siblings('.pDealyinvoice').val() == 0;
        input.val(0);
    }
});

//delay checkbox function
//$('.delayChk').change(function () {
//    var checked = $(this).is('checked');
//    $(this).siblings('.pDealyinvoice').val(checked ? 1 : 0);
//});


//function ToggleFilterDelay(divObj) {
//    var image = $(divObj).children("img");
//    var input = $(divObj).children('input');
//    if (input.val() == '0') {
//        image.attr('src', 'img/delay_false.png');
//        input.val(1);
//    }
//    else {
//        image.attr('src', 'img/delay_true.png');
//        input.val(0);
//    }
//};


//Show internal comment and hide siblings
$('.col > input').focus(function () {
    var timerow = $(this).parents('.timerow');
    timerow.find('.toggle').show(200);
    timerow.siblings().find('.toggle').hide(200);
});

$('#toggleMenu').click(function () {
    $('.sidebar').toggle("slide");
    //$('.sidebar').css("display", "block");
    //var div = $(".sidebar");
    //div.animate({ width: '280px' }, "slow");
    //div.animate({ height: '670px' }, "slow");
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
