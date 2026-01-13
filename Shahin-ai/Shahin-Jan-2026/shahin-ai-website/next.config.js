/** @type {import('next').NextConfig} */
const nextConfig = {
  output: 'standalone',
  images: {
    domains: ['portal.shahin-ai.com', 'app.shahin-ai.com'],
    formats: ['image/webp', 'image/avif'],
  },
  async redirects() {
    // Standardized on portal.shahin-ai.com to match ASP.NET backend
    const loginUrl = process.env.NEXT_PUBLIC_PORTAL_URL 
      ? `${process.env.NEXT_PUBLIC_PORTAL_URL}/Account/Login`
      : 'https://portal.shahin-ai.com/Account/Login';
    
    return [
      {
        source: '/login',
        destination: loginUrl,
        permanent: true,
      },
    ];
  },
  i18n: {
    locales: ['en', 'ar'],
    defaultLocale: 'ar',
    localeDetection: true,
  },
  // Suppress development warnings
  reactStrictMode: true,
  // Suppress webpack warnings in development
  webpack: (config, { dev, isServer }) => {
    if (dev && !isServer) {
      config.ignoreWarnings = [
        { module: /webpack-internal/ },
        { file: /webpack-internal/ },
      ];
    }
    return config;
  },
};

module.exports = nextConfig;
