import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';

// Import translation files
import enCommon from '../../public/locales/en/common.json';
import enDashboard from '../../public/locales/en/dashboard.json';
import enControls from '../../public/locales/en/controls.json';
import enEvidence from '../../public/locales/en/evidence.json';
import enReports from '../../public/locales/en/reports.json';
import enRemediation from '../../public/locales/en/remediation.json';
import enAuth from '../../public/locales/en/auth.json';

import arCommon from '../../public/locales/ar/common.json';
import arDashboard from '../../public/locales/ar/dashboard.json';
import arControls from '../../public/locales/ar/controls.json';
import arEvidence from '../../public/locales/ar/evidence.json';
import arReports from '../../public/locales/ar/reports.json';
import arRemediation from '../../public/locales/ar/remediation.json';
import arAuth from '../../public/locales/ar/auth.json';

const resources = {
  en: {
    common: enCommon,
    dashboard: enDashboard,
    controls: enControls,
    evidence: enEvidence,
    reports: enReports,
    remediation: enRemediation,
    auth: enAuth,
  },
  ar: {
    common: arCommon,
    dashboard: arDashboard,
    controls: arControls,
    evidence: arEvidence,
    reports: arReports,
    remediation: arRemediation,
    auth: arAuth,
  },
};

i18n
  .use(initReactI18next)
  .init({
    resources,
    lng: 'en',
    fallbackLng: 'en',
    defaultNS: 'common',
    interpolation: {
      escapeValue: false,
    },
    react: {
      useSuspense: false,
    },
  });

export default i18n;
