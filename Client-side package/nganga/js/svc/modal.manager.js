// This is a bit "un-Angular," but we're confining it to one place... ui-bootstrap offers some options here but they
// aren't really satisfactory for what I want to do.

ngangaUi.factory('modalManagerService', [function () {
  return {
    closeModal: function (modalName) {
      $('#' + modalName).modal('hide');
    },
    closeAllModals: function () {
      $('.modal').modal('hide');
    },
    showModal: function (modalName, onClose) {
      if (onClose) {
        $('#' + modalName).on('hidden.bs.modal', onClose);
      }
      $('#' + modalName).modal('show');
    }
  }
}]);