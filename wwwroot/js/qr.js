window.addEventListener('load', () => {
  try {
    const dataEl = document.getElementById('qrCodeData');
    const target = document.getElementById('qrCode');
    if (!dataEl || !target) return;

    const uri = dataEl.getAttribute('data-url');
    if (!uri || !uri.trim()) return;
    if (typeof QRCode !== 'function') return;

    while (target.firstChild) target.removeChild(target.firstChild);
    new QRCode(target, {
      text: uri,
      width: 150,
      height: 150,
      correctLevel: QRCode.CorrectLevel ? QRCode.CorrectLevel.M : undefined
    });
  } catch (e) {
    if (console && console.warn) console.warn('QR render error:', e);
  }
});