$(document).on("input", ".numeric", function () {
    this.value = this.value.replace(/\D/g, '');
});