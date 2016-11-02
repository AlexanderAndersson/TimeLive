﻿/// <reference path="moment.js" />
/// <reference path="moment.js" />

$(document).ready(function () {
    $('#month').fullCalendar({
        theme: true,
        header: {
            editable: false,
            left: '',
            center: 'prev title next',
            right: ''
        },
        themeButtonIcons: {
           
        },
        defaultView: 'month',
        editable: false,
        allDaySlot: true,
        selectable: true,
        firstDay: 1,
        fixedWeekCount: false,
        weekNumbers: true,
        weekends: false,
        height: "auto",
        navLinks: true,
        navLinkWeekClick: function (date) {
            $('#week').fullCalendar('gotoDate', date);
            $('#week').fullCalendar('changeView', 'agendaWeek');
        },
        navLinkDayClick: function (date) {
            $('#week').fullCalendar('gotoDate', date);
            $('#week').fullCalendar('changeView', 'agendaWeek');
        },

    });//Month
    $('#week').fullCalendar({
        theme: true,
        header: false,
        defaultView: 'basicWeek',
        editable: false,
        allDaySlot: false,
        selectable: true,
        firstDay: 1,
        fixedWeekCount: false,
        weekNumbers: true,
        weekends: false,
        //height: "auto",
        columnFormat: 'ddd',
        contentHeight: 300,
        //businessHours: true,
        //businessHours: { // specify an array instead

        //dow: [ 1, 2, 3, 4, 5 ], // Monday, Tuesday, Wednesday
        //start: '01:00', // 8am
        //end: '09:00', // 6pm
        //},
        //minTime: '01:00',
        //maxTime: '09:00',


    });//Week
});//document.ready


$('#btnInit').click(function () {
    $.ajax({
        type: 'POST',
        url: "/Time/GetKalender",
        success: function (response) {
            if (response == 'True') {
                $('#calendar').fullCalendar('refetchEvents');
                alert('Database populated! ');
            }
            else {
                alert('Error, could not populate database!');
            }
        }
    });
});

//Increase/Decrease numerics with arrow keys
$('.amount').keydown(function (event) {
    var currentNumber = Number($(this).val());
    if (event.which === 38)//up
    {
        if (currentNumber % 1 == 0 || currentNumber % 1 == 0.5)
            $(this).val(Math.max(0, currentNumber + 0.5).toFixed(2));
        else
            $(this).val(Math.max(0, Math.ceil(currentNumber * 2)) / 2)
    }       
    else if (event.which === 40)//down
    {
        if (currentNumber % 1 == 0 || currentNumber % 1 == 0.5)
            $(this).val(Math.max(0, currentNumber - 0.5).toFixed(2));
        else
            $(this).val(Math.floor(currentNumber * 2) / 2);
    }
});

//Increase/Decrease numerics with arrow keys
$('.amount-ints').keydown(function (event) {
    var currentNumber = Number($(this).val());
    if (event.which === 38)//up
    {
        if (currentNumber % 1 == 0)
            $(this).val(Math.max(0, currentNumber + 1).toFixed(0));
        else
            $(this).val(Math.max(0, Math.ceil(currentNumber)))
    }
    else if (event.which === 40)//down
    {
        if (currentNumber % 1 == 0)
            $(this).val(Math.max(0, currentNumber - 1).toFixed(0));
        else
            $(this).val(Math.floor(currentNumber));
    }
});


//if invoice or used are more than 24, make backgound-color "red" otherwise "green"
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
    autoSize: true
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


//delay checkbox function
$('.delayChk').change(function () {
    var checked = $(this).is(':checked');
    $(this).siblings('.pDealyinvoice').val(checked ? 1 : 0);
});


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

$('#quickCWeek').click(function () {
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
