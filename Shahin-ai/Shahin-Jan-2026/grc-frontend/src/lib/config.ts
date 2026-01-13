/**
 * Centralized URL Configuration for grc-frontend
 * 
 * All URLs should be loaded from environment variables.
 * Analytics URLs are optional - components should handle missing URLs gracefully.
 */

// API Configuration
export const API_CONFIG = {
  BASE_URL: process.env.NEXT_PUBLIC_API_URL || 
    (process.env.NODE_ENV === 'production' 
      ? 'https://portal.shahin-ai.com' 
      : 'http://localhost:8080'),
} as const;

// Analytics URLs (optional - components should check if undefined)
export const ANALYTICS_CONFIG = {
  SUPERSET_URL: process.env.NEXT_PUBLIC_SUPERSET_URL || undefined,
  GRAFANA_URL: process.env.NEXT_PUBLIC_GRAFANA_URL || undefined,
  METABASE_URL: process.env.NEXT_PUBLIC_METABASE_URL || undefined,
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
};
