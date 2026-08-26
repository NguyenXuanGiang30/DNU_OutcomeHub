import React, { useState } from 'react';
import { Outlet } from 'react-router-dom';
import { Sidebar } from './Sidebar';
import { Topbar } from './Topbar';
import { AiChatbotDrawer } from '../ai/AiChatbotDrawer';

export const Layout: React.FC = () => {
  const [aiDrawerOpen, setAiDrawerOpen] = useState(false);

  return (
    <div className="app-container">
      {/* Fixed Navigation Sidebar */}
      <Sidebar />

      {/* Fixed Header Topbar */}
      <Topbar onToggleAiDrawer={() => setAiDrawerOpen((prev) => !prev)} />

      {/* Main Routed Page Content */}
      <main className="main-content">
        <Outlet />
      </main>

      {/* Floating AI OBE Assistant Drawer */}
      <AiChatbotDrawer isOpen={aiDrawerOpen} onClose={() => setAiDrawerOpen(false)} />
    </div>
  );
};
