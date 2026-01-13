'use client';

import { motion } from 'framer-motion';
import { Shield, Lock, Eye, Database, UserCheck, Mail } from 'lucide-react';

export default function PrivacyPage() {
  return (
    <main className="min-h-screen bg-gradient-to-b from-slate-50 to-white">
      {/* Header */}
      <section className="py-20 bg-gradient-to-br from-slate-900 via-emerald-900 to-slate-900 text-white">
        <div className="container mx-auto px-6 text-center">
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
          >
            <div className="inline-flex items-center gap-2 px-4 py-2 bg-emerald-500/20 rounded-full text-emerald-300 mb-6">
              <Shield className="w-4 h-4" />
              <span>Your Privacy Matters</span>
            </div>
            <h1 className="text-4xl md:text-5xl font-bold mb-4">Privacy Policy</h1>
            <p className="text-xl text-slate-300 max-w-2xl mx-auto">
              سياسة الخصوصية - Privacy Policy
            </p>
            <p className="text-sm text-slate-400 mt-4">Last updated: January 2026</p>
          </motion.div>
        </div>
      </section>

      {/* Content */}
      <section className="py-16">
        <div className="container mx-auto px-6 max-w-4xl">
          <div className="prose prose-lg prose-slate max-w-none">
            
            <motion.div
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              className="mb-12"
            >
              <div className="flex items-center gap-3 mb-4">
                <Lock className="w-6 h-6 text-emerald-600" />
                <h2 className="text-2xl font-bold text-slate-900 m-0">1. Introduction</h2>
              </div>
              <p className="text-slate-600">
                Shahin AI (&quot;we&quot;, &quot;our&quot;, or &quot;us&quot;) is committed to protecting your privacy. This Privacy Policy explains how we collect, use, disclose, and safeguard your information when you use our GRC (Governance, Risk, and Compliance) platform.
              </p>
              <p className="text-slate-600 mt-4" dir="rtl">
                شاهين للذكاء الاصطناعي ملتزمة بحماية خصوصيتك. توضح سياسة الخصوصية هذه كيفية جمع واستخدام والإفصاح عن معلوماتك وحمايتها عند استخدام منصة الحوكمة والمخاطر والامتثال الخاصة بنا.
              </p>
            </motion.div>

            <motion.div
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              className="mb-12"
            >
              <div className="flex items-center gap-3 mb-4">
                <Database className="w-6 h-6 text-emerald-600" />
                <h2 className="text-2xl font-bold text-slate-900 m-0">2. Information We Collect</h2>
              </div>
              <ul className="list-disc pl-6 text-slate-600 space-y-2">
                <li><strong>Account Information:</strong> Name, email, company name, phone number</li>
                <li><strong>Usage Data:</strong> Log files, device information, IP addresses</li>
                <li><strong>Compliance Data:</strong> Documents, controls, evidence uploaded to the platform</li>
                <li><strong>Communication Data:</strong> Support tickets, feedback, correspondence</li>
              </ul>
            </motion.div>

            <motion.div
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              className="mb-12"
            >
              <div className="flex items-center gap-3 mb-4">
                <Eye className="w-6 h-6 text-emerald-600" />
                <h2 className="text-2xl font-bold text-slate-900 m-0">3. How We Use Your Information</h2>
              </div>
              <ul className="list-disc pl-6 text-slate-600 space-y-2">
                <li>Provide and maintain our GRC platform services</li>
                <li>Process transactions and send related information</li>
                <li>Send administrative information and updates</li>
                <li>Respond to inquiries and offer customer support</li>
                <li>Improve our services and develop new features</li>
                <li>Comply with legal obligations</li>
              </ul>
            </motion.div>

            <motion.div
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              className="mb-12"
            >
              <div className="flex items-center gap-3 mb-4">
                <UserCheck className="w-6 h-6 text-emerald-600" />
                <h2 className="text-2xl font-bold text-slate-900 m-0">4. Your Rights</h2>
              </div>
              <p className="text-slate-600">You have the right to:</p>
              <ul className="list-disc pl-6 text-slate-600 space-y-2 mt-2">
                <li>Access your personal data</li>
                <li>Correct inaccurate data</li>
                <li>Request deletion of your data</li>
                <li>Object to processing of your data</li>
                <li>Data portability</li>
                <li>Withdraw consent at any time</li>
              </ul>
            </motion.div>

            <motion.div
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              className="mb-12"
            >
              <div className="flex items-center gap-3 mb-4">
                <Mail className="w-6 h-6 text-emerald-600" />
                <h2 className="text-2xl font-bold text-slate-900 m-0">5. Contact Us</h2>
              </div>
              <p className="text-slate-600">
                For privacy-related inquiries, please contact us at:
              </p>
              <div className="bg-slate-100 p-4 rounded-lg mt-4">
                <p className="text-slate-700 m-0">
                  <strong>Email:</strong> privacy@shahin-ai.com<br />
                  <strong>Address:</strong> Riyadh, Saudi Arabia
                </p>
              </div>
            </motion.div>

          </div>
        </div>
      </section>
    </main>
  );
}
