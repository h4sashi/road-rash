mergeInto(LibraryManager.library, {
  ShareImageWithURL: function (imageData, url) {
    let gameURL = UTF8ToString(url);
    let base64String = UTF8ToString(imageData);

    console.log('Attempting to share via social media...');

    // Convert base64 to Blob
    let byteCharacters = atob(base64String);
    let byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
      byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    let byteArray = new Uint8Array(byteNumbers);
    let blob = new Blob([byteArray], { type: 'image/png' });

    // Convert Blob to Object URL
    let imageObjectURL = URL.createObjectURL(blob);

    // Create a share message
    let shareMessage = encodeURIComponent('Check out this game! ' + gameURL);

    // Open a social media share link
    let twitterURL = `https://twitter.com/intent/tweet?text=${shareMessage}`;
    let whatsappURL = `https://api.whatsapp.com/send?text=${shareMessage}`;
    let telegramURL = `https://t.me/share/url?url=${gameURL}&text=${shareMessage}`;

    let shareOptions = `
            <html>
            <head>
                <title>Share This Game</title>
                <style>
                    body {
                        font-family: Arial, sans-serif;
                        text-align: center;
                        padding: 20px;
                        background-color: #f8f9fa;
                        color: #333;
                    }
                    h2 {
                        font-size: 22px;
                        margin-bottom: 10px;
                    }
                    img {
                        width: 80%;
                        border-radius: 10px;
                        box-shadow: 0px 4px 6px rgba(0,0,0,0.1);
                        margin-bottom: 10px;
                    }
                    .btn {
                        display: block;
                        width: 80%;
                        padding: 10px;
                        margin: 10px auto;
                        text-decoration: none;
                        font-size: 18px;
                        font-weight: bold;
                        border-radius: 5px;
                        text-align: center;
                    }
                    .twitter { background-color: #1DA1F2; color: white; }
                    .whatsapp { background-color: #25D366; color: white; }
                    .telegram { background-color: #0088cc; color: white; }
                    .download { background-color: #ff9800; color: white; }
                </style>
            </head>
            <body>
                <h2>Share This Game</h2>
                <img src='${imageObjectURL}' alt='Game Screenshot'>
                <a href='${twitterURL}' target='_blank' class='btn twitter'>🐦 Share on Twitter</a>
                <a href='${whatsappURL}' target='_blank' class='btn whatsapp'>📱 Share on WhatsApp</a>
                <a href='${telegramURL}' target='_blank' class='btn telegram'>💬 Share on Telegram</a>
                <a href='${imageObjectURL}' download='screenshot.png' class='btn download'>📥 Download Screenshot</a>
            </body>
            </html>
        `;

    let popupWidth = 450,
      popupHeight = 500;
    let left = (screen.width - popupWidth) / 2;
    let top = (screen.height - popupHeight) / 2;

    let shareWindow = window.open(
      '',
      '_blank',
      `width=${popupWidth},height=${popupHeight},top=${top},left=${left},resizable=yes,scrollbars=yes`
    );

    if (!shareWindow) {
      alert('Pop-up blocked! Please allow pop-ups for this site.');
      return;
    }

    shareWindow.document.write(shareOptions);
  },
});
