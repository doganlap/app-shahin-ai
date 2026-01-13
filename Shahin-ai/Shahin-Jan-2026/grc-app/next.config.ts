import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  // Standardized configuration for grc-app
  output: 'standalone',

  images: {
    remotePatterns: [
      {
        protocol: 'https',
        hostname: 'portal.shahin-ai.com',
      },
      {
        protocol: 'https',
        hostname: 'app.shahin-ai.com',
      },
      {
        protocol: 'https',
        hostname: 'shahin-ai.com',
      },
    ],
    formats: ['image/webp', 'image/avif'],
  },

  // Environment variable validation
  env: {
    NEXT_PUBLIC_API_URL: process.env.NEXT_PUBLIC_API_URL || 'https://portal.shahin-ai.com',
  },

  // Redirects - standardized on portal.shahin-ai.com (matches ASP.NET backend)
  async redirects() {
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
};

export default nextConfig;
