'use client';

import Link from "next/link";
import { useTranslation } from 'react-i18next';
import LanguageSelector from '@/components/LanguageSelector';

export default function Home() {
  const { t } = useTranslation('landing');

  return (
    <div className="min-h-screen bg-gradient-to-br from-[#0B1F3B] to-[#0E7490]">
      {/* Header with Language Selector */}
      <header className="absolute top-0 right-0 p-6 z-10">
        <LanguageSelector />
      </header>

      {/* Hero Section */}
      <div className="flex min-h-screen items-center justify-center px-4">
        <main className="max-w-4xl text-center text-white">
          <div className="mb-8">
            <div className="inline-flex items-center justify-center w-20 h-20 bg-white rounded-2xl shadow-xl mb-6">
              <span className="text-4xl font-bold text-[#0E7490]">ش</span>
            </div>
            <h1 className="text-xl font-bold mb-2">Shahin AI</h1>
          </div>

          <h2 className="text-5xl font-bold mb-6">{t('hero.title')}</h2>
          <p className="text-2xl mb-4 text-white/90">{t('hero.subtitle')}</p>
          <p className="text-lg mb-12 text-white/80 max-w-2xl mx-auto">{t('hero.description')}</p>

          <div className="flex flex-col sm:flex-row gap-4 justify-center">
            <Link
              href="/login"
              className="px-8 py-4 bg-white text-[#0E7490] font-semibold rounded-lg hover:bg-gray-100 transition-colors"
            >
              {t('hero.cta.getStarted')}
            </Link>
            <Link
              href="/login"
              className="px-8 py-4 border-2 border-white text-white font-semibold rounded-lg hover:bg-white/10 transition-colors"
            >
              {t('hero.cta.requestDemo')}
            </Link>
          </div>

          {/* Frameworks */}
          <div className="mt-16">
            <p className="text-sm text-white/60 mb-4">{t('frameworks.subtitle')}</p>
            <div className="flex flex-wrap justify-center gap-4">
              <div className="px-4 py-2 bg-white/10 rounded-lg backdrop-blur-sm">NCA ECC</div>
              <div className="px-4 py-2 bg-white/10 rounded-lg backdrop-blur-sm">SAMA CSF</div>
              <div className="px-4 py-2 bg-white/10 rounded-lg backdrop-blur-sm">PDPL</div>
              <div className="px-4 py-2 bg-white/10 rounded-lg backdrop-blur-sm">NCA CSCC</div>
            </div>
          </div>
        </main>
      </div>
    </div>
  );
}
