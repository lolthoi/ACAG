import"./esm-chunk-d81494b9.js";import{ensureElement as e,elementIsInDOM as t}from"./esm-dom-utils-c40cd314.js";import{a as o,d as n}from"./esm-chunk-9c16a801.js";import{d as s,r}from"./esm-chunk-f3c18c5b.js";import{T as c}from"./esm-chunk-1b6abd73.js";import{K as u}from"./esm-chunk-808bf349.js";import{getDropDownElement as i,onOutsideClick as a,isDropDownVisible as d}from"./esm-dropdowns-990ef398.js";import"./esm-chunk-13e2cf5f.js";import{scrollToFocusedItem as m,getParametersForVirtualScrollingRequest as l}from"./esm-grid-1b060bef.js";import"./esm-grid-column-resize-adeb3acf.js";import"./esm-popup-utils-32033e18.js";import"./esm-focus-utils-c767d78c.js";import"./esm-chunk-d4fc448c.js";import"./esm-dragAndDropUnit-f02dc664.js";import{k as f,a as p}from"./esm-chunk-30985063.js";function k(e){const t=i(e);m(t)}function y(t){t=e(t),document.activeElement===t&&b(t)}function C(e,t){e.dataset.timerId&&(clearTimeout(e.dataset.timerId),delete e.dataset.timerId),t||setTimeout((function(){C(e,!0)}),100)}function b(e){e&&e.dataset.focusedClass&&(e.className=e.dataset.focusedClass)}function g(e,t,o,n,s){j(e)&&(e.stopPropagation(),e.preventDefault());const r=function(e){const t=e.altKey&&(e.keyCode===u.Down||e.keyCode===u.Up),o=h(e),n=e.keyCode===u.Esc||e.keyCode===u.Enter;return t||o||n}(e)&&!e.repeat;n&&!e.repeat&&f(e),r&&v(e,t,o,s)}function v(e,t,o,n){const s=e.target.value;if(o&&n){const n=o.querySelector(".dxgvCSD");n&&(o=n);const r=l(o,!1);t.invokeMethodAsync("ComboBoxVirtualScrollingProcessKey",s,e.keyCode,e.altKey,e.ctrlKey,r.requestScrollState.itemHeight,r.requestScrollState.scrollTop,r.requestScrollState.scrollHeight)}else t.invokeMethodAsync("ComboBoxProcessKey",s,e.keyCode,e.altKey,e.ctrlKey)}function h(e){return e.keyCode===u.Down||e.keyCode===u.Up||e.keyCode===u.PageUp||e.keyCode===u.PageDown||e.ctrlKey&&(e.keyCode===u.Home||e.keyCode===u.End)}function j(e){return h(e)||e.keyCode===u.Enter}function w(e,t,o){if(!t)return;const n=(new Date).getTime();t.dataset.lastLostFocusTime&&n-t.dataset.lastLostFocusTime<300&&!o||(t.dataset.lastLostFocusTime=(new Date).getTime(),e.invokeMethodAsync("OnComboBoxLostFocus",t.value))}function E(m,l,f){const k=e(m);if(!k)return;s(k);const y=l.filteringEnabled,h=e(l.inputElement),E=i(k),D=E,S=function(e){return g(e,f,D,y,l.virtualScrollingEnabled)},T=function(e){return function(e,t,o,n,s){return j(e)?(e.stopPropagation(),e.preventDefault(),e.keyCode!==u.Up&&e.keyCode!==u.Down||t.invokeMethodAsync("OnComboBoxKeyUp",e.keyCode===u.Up)):n&&p(e)&&v(e,t,o,s),!1}(e,f,D,y,l.virtualScrollingEnabled)},K=function(e){return function(e,t){b(e.target)}(e)},I=function(e){return function(e,t){const o=e.target;C(o,!0),o.dataset.timerId=setTimeout((function(){if(delete o.dataset.timerId,function(e){e&&e.dataset.bluredClass&&(e.className=e.dataset.bluredClass)}(o),document.activeElement!==o)try{w(t,o)}catch(e){}}),200)}(e,f)},P=function(e){return C(h)},x=function(e){return a(e,k,(function(){t(k)||s(k);const e=document.activeElement===h,o=h.dataset.timerId>0,n=E&&d(E);C(h),(e||o||n)&&w(f,h,!0)}))};return o(h,"keydown",S),o(h,"keyup",T),o(h,"focus",K),o(h,"blur",I),o(k,"mousedown",P),o(document,c.touchMouseDownEventName,x),r(k,(function(){n(h,"keydown",S),n(h,"keyup",T),n(h,"focus",K),n(h,"blur",I),n(k,"mousedown",P),n(document,c.touchMouseDownEventName,x)})),Promise.resolve("ok")}function D(t){if(t=e(t))return s(t),Promise.resolve("ok")}const S={init:E,dispose:D,prepareInputIfFocused:y,scrollToSelectedItem:k};export default S;export{D as dispose,E as init,y as prepareInputIfFocused,k as scrollToSelectedItem};