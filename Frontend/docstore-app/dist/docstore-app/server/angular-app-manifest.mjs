
export default {
  bootstrap: () => import('./main.server.mjs').then(m => m.default),
  inlineCriticalCss: true,
  baseHref: '/',
  locale: undefined,
  routes: undefined,
  entryPointToBrowserMapping: {},
  assets: {
    'index.csr.html': {size: 3650, hash: '9ddad0e6aec1e4dd6cb87afc4ccd220810bdf089b92b4363765858632fcb5ced', text: () => import('./assets-chunks/index_csr_html.mjs').then(m => m.default)},
    'index.server.html': {size: 1008, hash: 'b7e8375f85710e4a9aaf498f8adc43963b069a950a33d09a6236c14664676b48', text: () => import('./assets-chunks/index_server_html.mjs').then(m => m.default)},
    'styles-GYJMHD2O.css': {size: 3030, hash: 'iGR/Jz5sAIo', text: () => import('./assets-chunks/styles-GYJMHD2O_css.mjs').then(m => m.default)}
  },
};
