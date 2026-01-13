/**
 * Centralized URL Configuration
 * 
 * All URLs should be loaded from environment variables.
 * Fallback values are provided for development only.
 */

// API Configuration
export const API_CONFIG = {
  // Backend API URL - Standardized on portal.shahin-ai.com (matches ASP.NET backend)
  BASE_URL: process.env.NEXT_PUBLIC_API_URL || 
    (process.env.NODE_ENV === 'production' 
      ? 'https://portal.shahin-ai.com' 
      : 'http://localhost:8080'),
  
  // Portal URL - For authentication and app access
  PORTAL_URL: process.env.NEXT_PUBLIC_PORTAL_URL || 
    (process.env.NODE_ENV === 'production'
      ? 'https://portal.shahin-ai.com'
      : 'http://localhost:8080'),
  
  // Login URL - Standardized path
  LOGIN_URL: (process.env.NEXT_PUBLIC_PORTAL_URL || 
    (process.env.NODE_ENV === 'production'
      ? 'https://portal.shahin-ai.com'
      : 'http://localhost:8080')) + '/Account/Login',
} as const;

// Analytics URLs (optional - only if analytics are enabled)
export const ANALYTICS_CONFIG = {
  SUPERSET_URL: process.env.NEXT_PUBLIC_SUPERSET_URL || undefined,
  GRAFANA_URL: process.env.NEXT_PUBLIC_GRAFANA_URL || undefined,
  METABASE_URL: process.env.NEXT_PUBLIC_METABASE_URL || undefined,
} as const;

// Landing/Marketing URLs
export const MARKETING_CONFIG = {
  LANDING_URL: process.env.NEXT_PUBLIC_LANDING_URL || 'https://shahin-ai.com',
  WEBSITE_URL: process.env.NEXT_PUBLIC_WEBSITE_URL || 'https://shahin-ai.com',
} as const;

/**
 * Validate required environment variables in production
 */
if (process.env.NODE_ENV === 'production') {
  const requiredVars = ['NEXT_PUBLIC_API_URL'];
  const missing = requiredVars.filter(v => !process.env[v]);
  
  if (missing.length > 0) {
    console.warn(
      `⚠️ Missing required environment variables: ${missing.join(', ')}\n` +
      `Using fallback values. This should be fixed before deployment.`
    );
  }
}

export default {
  api: API_CONFIG,
  analytics: ANALYTICS_CONFIG,
  marketing: MARKETING_CONFIG,
};
