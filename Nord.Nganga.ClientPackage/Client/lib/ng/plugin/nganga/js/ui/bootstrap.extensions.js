// You can find lots of variations on this idea online, but I used this one:
// http://stackoverflow.com/a/24914782/2056448

$(document).on('show.bs.modal', '.modal', function()
{
  var zIndex = 100 + (10 * $('.modal:visible').length);
  $(this).css('z-index', zIndex);
  setTimeout(function()
  {
    $('.modal-backdrop').not('.modal-stack').css('z-index', zIndex - 1).addClass('modal-stack');
  }, 0)
});

$(document).on('hidden.bs.modal', '.modal', function()
{
  if ($('.modal:visible').length)
  {
    $(document.body).addClass('modal-open');
  }
});

$(window).on('popstate', function()
{
  $('.modal').modal('hide');
  $('body').removeClass('modal-open');
  $('.modal-backdrop').hide();
});