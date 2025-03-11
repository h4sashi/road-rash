mergeInto(LibraryManager.library, {
  SaveScreenshot: function (data, length) {
    var uint8Array = new Uint8Array(Module.HEAPU8.buffer, data, length);
    var blob = new Blob([uint8Array], { type: 'image/png' });
    var url = URL.createObjectURL(blob);

    var a = document.createElement('a');
    a.href = url;
    a.download = 'screenshot.png';
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
  },
});
