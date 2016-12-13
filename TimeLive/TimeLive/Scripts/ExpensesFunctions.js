$('#toggleMenu').click(function () { //Toggle filter in navbar
    $('#filter').toggle("slide");
});

//reimbuse checkbox function
$('.reimChk').change(function () {
    var checked = $(this).is(':checked');
    $(this).siblings('.pReimburse').val(checked ? 1 : 0);
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

//Changes picture when clicking on delay img
$('.reimChk').on('click', function () {
    var checked = $(this).is(':checked');
    var image = $(this).siblings("img");
    var input = $(this).siblings('.reimburse');
    if (checked == false) {
        image.attr('src', '/img/reimburse_false.png');
        input.val(0);
        $(this).removeClass('check');
    }
    else {
        image.attr('src', '/img/reimburse_true.png');
        input.val(1);
        $(this).addClass('check');
    }
});

//Makes so that when you reload the page, the right image is showned on delays.
$('.reimChk').each(function () {
    var checked = $(this).is(':checked');
    var image = $(this).siblings("img");

    if (checked == true) {
        image.attr('src', '/img/reimburse_true.png');
        $(this).addClass('check');
    }
    else {
        image.attr('src', '/img/reimburse_false.png');
        $(this).removeClass('check');
    }
});