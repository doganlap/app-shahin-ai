export const environment = {
  production: false,
  application: {
    name: 'GRC Platform',
    logoUrl: '',
  },
  oAuthConfig: {
    issuer: 'http://localhost:7000',
    clientId: 'Grc_App',
    dummyClientSecret: '1q2w3e*',
    scope: 'offline_access Grc',
    showDebugInformation: true,
    oidc: false,
    requireHttps: false,
  },
  apis: {
    default: {
      url: 'http://localhost:7000',
      rootNamespace: 'Grc',
    },
  },
};
