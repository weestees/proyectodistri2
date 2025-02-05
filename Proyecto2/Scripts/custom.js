$(document).ready(function () {
  // Reemplaza .andSelf() con .addBack()
  $("selector").parents().addBack().css("color", "red");
  // ...existing code...
});
