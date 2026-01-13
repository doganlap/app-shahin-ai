'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { useTranslation } from 'react-i18next';

export default function Sidebar() {
  const pathname = usePathname();
  const { t } = useTranslation('common');

  const menuItems = [
    { name: t('navigation.dashboard'), href: '/dashboard', icon: '📊' },
    { name: t('navigation.controls'), href: '/controls', icon: '🛡️', badge: 'MAP' },
    { name: t('navigation.requirements'), href: '/requirements', icon: '📋' },
    { name: t('navigation.applicability'), href: '/applicability', icon: '🎯', badge: 'APPLY' },
    { name: t('navigation.evidence'), href: '/evidence', icon: '📁', badge: 'PROVE' },
    { name: t('navigation.tests'), href: '/tests', icon: '🧪' },
    { name: t('navigation.reports'), href: '/reports', icon: '📈', badge: 'WATCH' },
    { name: t('navigation.remediation'), href: '/remediation', icon: '🔧', badge: 'FIX' },
    { name: t('navigation.vault'), href: '/vault', icon: '🔒', badge: 'VAULT' },
    { name: t('navigation.assessments'), href: '/assessments', icon: '✅' },
    { name: t('navigation.risks'), href: '/risks', icon: '⚠️' },
    { name: t('navigation.audits'), href: '/audits', icon: '🔍' },
  ];

  const adminItems = [
    { name: t('navigation.settings'), href: '/settings', icon: '⚙️' },
    { name: t('navigation.users'), href: '/users', icon: '👥' },
    { name: t('navigation.tenants'), href: '/tenants', icon: '🏢' },
  ];

  return (
    <aside className="w-64 bg-[#0B1F3B] text-white min-h-screen flex flex-col">
      {/* Logo */}
      <div className="p-4 border-b border-white/10">
        <Link href="/dashboard" className="flex items-center space-x-3">
          <div className="w-10 h-10 bg-gradient-to-br from-[#0E7490] to-[#14B8A6] rounded-lg flex items-center justify-center">
            <span className="text-white font-bold text-xl">ش</span>
          </div>
          <div>
            <span className="text-lg font-bold">{t('app.name')}</span>
            <span className="block text-xs text-gray-400">{t('app.title')}</span>
          </div>
        </Link>
      </div>

      {/* Main Menu */}
      <nav className="flex-1 p-4 space-y-1 overflow-y-auto">
        <div className="text-xs uppercase text-gray-500 mb-2 px-3">{t('navigation.mainMenu')}</div>
        {menuItems.map((item) => {
          const isActive = pathname === item.href || pathname?.startsWith(item.href + '/');
          return (
            <Link
              key={item.href}
              href={item.href}
              className={`flex items-center justify-between px-3 py-2 rounded-lg transition-colors ${
                isActive
                  ? 'bg-[#0E7490] text-white'
                  : 'text-gray-300 hover:bg-white/10 hover:text-white'
              }`}
            >
              <span className="flex items-center space-x-3">
                <span>{item.icon}</span>
                <span>{item.name}</span>
              </span>
              {item.badge && (
                <span className="text-xs bg-white/20 px-2 py-0.5 rounded">{item.badge}</span>
              )}
            </Link>
          );
        })}

        <div className="text-xs uppercase text-gray-500 mt-6 mb-2 px-3">{t('navigation.admin')}</div>
        {adminItems.map((item) => {
          const isActive = pathname === item.href;
          return (
            <Link
              key={item.href}
              href={item.href}
              className={`flex items-center space-x-3 px-3 py-2 rounded-lg transition-colors ${
                isActive
                  ? 'bg-[#0E7490] text-white'
                  : 'text-gray-300 hover:bg-white/10 hover:text-white'
              }`}
            >
              <span>{item.icon}</span>
              <span>{item.name}</span>
            </Link>
          );
        })}
      </nav>

      {/* User Profile */}
      <div className="p-4 border-t border-white/10">
        <div className="flex items-center space-x-3">
          <div className="w-10 h-10 bg-[#0E7490] rounded-full flex items-center justify-center">
            <span className="text-white font-medium">AD</span>
          </div>
          <div className="flex-1">
            <div className="text-sm font-medium">{t('user.profile')}</div>
            <div className="text-xs text-gray-400">{t('user.email')}</div>
          </div>
          <Link href="/logout" className="text-gray-400 hover:text-white">
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
            </svg>
          </Link>
        </div>
      </div>
    </aside>
  );
}
