mergeInto(LibraryManager.library, {
  Hello: function () {
    window.alert("Hello, world!");
  },
  
  Initialization: function () {
    if (window.unityInstance) {
      console.log('Initialized');
    }
  },
  
  SendScore: function (score, game) {
    // score: Integer
    // game: Integer
    if (window.unityInstance) {
      window.parent.postMessage({
        type: 'SEND_SCORE',
        score: score,
        game: game
      }, "*"); // consider replacing "*" with specific origin in production
      console.log('Score message sent to parent');
    }
  },

InitializeOGP: function(gameIdPtr, playerIdPtr) {
  console.log('[OGP] InitializeOGP called. ptrs:', gameIdPtr, playerIdPtr);
  try {
    var gameId = null, playerId = null;

    // Safe pointer -> string conversions
    try {
      if (gameIdPtr && gameIdPtr !== 0) gameId = UTF8ToString(gameIdPtr);
    } catch (e) {
      console.error('[OGP] UTF8ToString failed for gameIdPtr:', gameIdPtr, e);
      if (window.unityInstance) window.unityInstance.SendMessage('OGPBridge','OnOGPError','utf8_gameId_failed');
      return;
    }
    try {
      if (playerIdPtr && playerIdPtr !== 0) playerId = UTF8ToString(playerIdPtr);
    } catch (e) {
      console.error('[OGP] UTF8ToString failed for playerIdPtr:', playerIdPtr, e);
      if (window.unityInstance) window.unityInstance.SendMessage('OGPBridge','OnOGPError','utf8_playerId_failed');
      return;
    }

    console.log('[OGP] parsed gameId, playerId:', gameId, playerId);

    // Ensure SDK is available
    if (typeof window.OpenGameSDK === 'undefined' && typeof window.OpenGame === 'undefined') {
      console.error('[OGP] OpenGameSDK not found. Add the CDN script to the page.');
      if (window.unityInstance) window.unityInstance.SendMessage('OGPBridge', 'OnOGPError', 'sdk_not_loaded');
      return;
    }

    // Construct instance if missing
    if (typeof window.ogp === 'undefined') {
      try {
        window.ogp = new OpenGameSDK({ ui: { usePointsWidget: true }, logLevel: 1 });
      } catch (e) {
        console.error('[OGP] Failed to construct OpenGameSDK:', e);
        if (window.unityInstance) window.unityInstance.SendMessage('OGPBridge', 'OnOGPError', 'constructor_failed');
        return;
      }
    }

    // Call init INSIDE the same try scope so variables are in scope
    window.ogp.init({
      gameId: gameId,
      playerId: playerId,
      metadata: { url: window.location.href }
    })
    .then(function() {
      console.log('[OGP] init successful for gameId:', gameId, 'playerId:', playerId);
      if (window.unityInstance) window.unityInstance.SendMessage('OGPBridge', 'OnOGPInit', 'ok');
    })
    .catch(function(err) {
      var msg = err && err.message ? err.message : JSON.stringify(err || 'init_failed');
      console.error('[OGP] init failed:', msg);
      if (window.unityInstance) window.unityInstance.SendMessage('OGPBridge', 'OnOGPError', msg);
    });

  } catch (e) {
    console.error('[OGP] InitializeOGP top-level exception', e);
    if (window.unityInstance) window.unityInstance.SendMessage('OGPBridge', 'OnOGPError', e.message || 'exception');
  }
},


  SavePoints: function(points) {
  // points will be a number (Emscripten passes numeric args)
  if (!window.ogp) {
    console.error('[OGP] savePoints called but ogp not initialized.');
    if (window.unityInstance) window.unityInstance.SendMessage('OGPBridge', 'OnOGPError', 'ogp_not_initialized');
    return;
  }
  try {
    // ensure numeric
    var pts = Number(points) || 0;
    console.log('[OGP] SavePoints called with:', pts);

    if (pts <= 0) {
      console.warn('[OGP] savePoints requires a positive number. Aborting.');
      if (window.unityInstance) window.unityInstance.SendMessage('OGPBridge', 'OnOGPError', 'invalid_points');
      return;
    }

    window.ogp.savePoints(pts)
      .then(function(total) {
        console.log('[OGP] savePoints resolved. totalPoints for the day:', total);
        if (window.unityInstance) window.unityInstance.SendMessage('OGPBridge', 'OnOGPSavePoints', String(total || 0));
      })
      .catch(function(err) {
        var msg = err && err.message ? err.message : JSON.stringify(err || 'save_failed');
        console.error('[OGP] savePoints failed:', msg);
        if (window.unityInstance) window.unityInstance.SendMessage('OGPBridge', 'OnOGPError', msg);
      });
  } catch (e) {
    console.error('[OGP] SavePoints exception:', e);
    if (window.unityInstance) window.unityInstance.SendMessage('OGPBridge', 'OnOGPError', e.message || 'exception');
  }
},

  StartCustomAuth: function() {
    if (window.ogp && typeof window.ogp.startCustomAuth === 'function') {
      try { window.ogp.startCustomAuth(); }
      catch (e) { console.warn('startCustomAuth threw', e); }
    } else {
      console.warn('startCustomAuth not available on ogp instance');
    }
  },

  SetCustomAuthToken: function(tokenPtr) {
    var token = UTF8ToString(tokenPtr);
    if (window.ogp && typeof window.ogp.setCustomAuthToken === 'function') {
      try { window.ogp.setCustomAuthToken(token); }
      catch (e) { console.warn('setCustomAuthToken threw', e); }
    } else {
      console.warn('setCustomAuthToken not available on ogp instance');
    }
  }
});
