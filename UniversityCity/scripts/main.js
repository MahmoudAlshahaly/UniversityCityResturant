$(document).ready(function () {
  'use strict';

    // Meals Slider
  $('#mealSlides').slick({
      dots: true,
      infinite: true,
      speed: 300,
      autoplay: true,
      arrows: false,
      slidesToShow: 3,
      slidesToScroll: 3,
      responsive: [
        {
            breakpoint: 1200,
            settings: {
                slidesToShow: 3,
                slidesToScroll: 3
            }
        },
        {
            breakpoint: 992,
            settings: {
                slidesToShow: 3,
                slidesToScroll: 3
            }
        },
        {
            breakpoint: 768,
            settings: {
                slidesToShow: 2,
                slidesToScroll: 2
            }
        },
        {
            breakpoint: 500,
            settings: {
                slidesToShow: 1,
                slidesToScroll: 1
            }
        }
      ]
  });


  // Navbar menu toggler button
  $('.navBar .navbar-toggler').click(function () {
		$(this).toggleClass('opened');
	});


  // حجز الكوبون
  // $('.timer').startTimer({
  //     classNames: {
  //     hours: 'cdHours',
  //     minutes: 'cdMinutes',
  //     seconds: 'cdSeconds'
  //   }
  // });

  var now = new Date($.now());
  var nowHrs = now.getHours();
  var nowMin = now.getMinutes();
  var nowSec = now.getSeconds();
  var currentTime = nowHrs + ":" + nowMin + ":" + nowSec;
  var start = '19:00:00';
  var end = currentTime;

  var s = start.split(':');
  var e = end.split(':');

  var sec = e[2] - s[2];
  var min_carry = 0;

  if (sec < 0) {
    sec += 60;
    min_carry += 1;
  }

  var min = e[1] - s[1];
  var hour_carry = 0;

  if (min < 0) {
    min += 60;
    hour_carry += 1;
  }

  var hour = e[0] - s[0] - hour_carry;
  var diff = hour + ":" + min + ":" + sec;

  var diffInNum = hour*60 + min*1 + sec/60;
  var remTime = 240 - diffInNum;
  // alert(remTime);

  $(function () {
    if (nowHrs >= 19 && nowHrs < 23) {
      $('#stTimer').attr('data-minutes-left', remTime);

      // alert('Less');
      $('#stTimer').startTimer({
          classNames: {
          hours: 'cdHours',
          minutes: 'cdMinutes',
          seconds: 'cdSeconds'
        }
      });
    } else {
      // alert('Big');
      $('#stTimer').attr('data-minutes-left', '0');
      $('#stTimer').startTimer({
          classNames: {
          hours: 'cdHours',
          minutes: 'cdMinutes',
          seconds: 'cdSeconds'
        }
      });

      $('#stTimer .cdHours, #stTimer .cdSeconds, #stTimer .cdMinutes').text('00')
    }
  });



  // $(function () {
  //   $('#stTimer').attr('data-minutes-left', remTime);
  //
  //   $('.timer').startTimer({
  //       classNames: {
  //       hours: 'cdHours',
  //       minutes: 'cdMinutes',
  //       seconds: 'cdSeconds'
  //     }
  //   });
  // });










	// آراء الدكاترة
	$('.quotes .slides').slick({
		infinite: true,
		speed:500,
		autoplay: true,
		autoplaySpeed: 10000,
		slidesToShow: 1,
		swipeToSlide: true,
		slidesToScroll: 1
	});


  // Date and time pickers
  $('.datetimepicker').datetimepicker({
    format: "dd/mm/yyyy - HH:ii P",
    showMeridian: true,
    autoclose: true,
    todayBtn: true
  });


  // Tabs group
  $('.tabsGroup .controller li').click(function() {
    var ref = $(this).attr('tab-target');
    $(this).addClass('active').siblings().removeClass('active');
    $('.tabsGroup .contents div[id='+ ref +']').addClass('show').siblings().removeClass('show');
  });



});
