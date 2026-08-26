import React, { useState, useEffect } from 'react';
import { NavLink, useLocation } from 'react-router-dom';
import {
  LayoutDashboard,
  Database,
  Network,
  BookOpen,
  ClipboardCheck,
  TrendingUp,
  Sparkles,
  ShieldCheck,
  ChevronDown,
  ChevronRight,
  Cpu,
  Search,
} from 'lucide-react';

export interface NavSubItem {
  path: string;
  label: string;
  badge?: string;
}

export interface NavGroup {
  id: string;
  label: string;
  icon: React.ComponentType<{ size?: number; className?: string }>;
  badge?: string;
  children?: NavSubItem[];
  path?: string;
}

export const navTree: NavGroup[] = [
  {
    id: 'dashboard',
    label: 'Tổng quan',
    icon: LayoutDashboard,
    path: '/',
  },
  {
    id: 'academic-data',
    label: 'Dữ liệu đào tạo',
    icon: Database,
    children: [
      { path: '/data/org-units', label: 'Đơn vị – Khoa' },
      { path: '/data/programs', label: 'Ngành đào tạo' },
      { path: '/data/cohorts', label: 'Khóa tuyển sinh' },
      { path: '/data/students', label: 'Sinh viên' },
      { path: '/data/courses', label: 'Học phần' },
    ],
  },
  {
    id: 'curriculum-outcomes',
    label: 'Chương trình & chuẩn đầu ra',
    icon: Network,
    badge: 'ABET',
    children: [
      { path: '/curriculum/programs', label: 'Chương trình đào tạo' },
      { path: '/curriculum/versions', label: 'Phiên bản CTĐT' },
      { path: '/curriculum/pos', label: 'Mục tiêu đào tạo – PO' },
      { path: '/curriculum/plos', label: 'Chuẩn đầu ra – PLO' },
      { path: '/curriculum/pis', label: 'Chỉ báo thực hiện – PI' },
      { path: '/curriculum/weight-a', label: 'Trọng số A' },
      { path: '/curriculum/clos', label: 'Chuẩn đầu ra học phần – CLO' },
      { path: '/curriculum/matrix', label: 'Ma trận liên kết' },
    ],
  },
  {
    id: 'syllabus-assessment',
    label: 'Đề cương và đánh giá',
    icon: BookOpen,
    badge: 'BM13',
    children: [
      { path: '/syllabus/bm13', label: 'Đề cương chi tiết học phần' },
      { path: '/syllabus/plans', label: 'Kế hoạch đánh giá' },
      { path: '/syllabus/blueprints', label: 'Đề thi – Bài đánh giá' },
      { path: '/syllabus/rubrics', label: 'Rubric' },
      { path: '/syllabus/approvals', label: 'Phê duyệt đề cương' },
      { path: '/syllabus/exam-approvals', label: 'Phê duyệt đề thi' },
    ],
  },
  {
    id: 'measurement-scoring',
    label: 'Đo lường chuẩn đầu ra',
    icon: ClipboardCheck,
    children: [
      { path: '/measurement/periods', label: 'Đợt đo lường' },
      { path: '/measurement/sources', label: 'Nguồn đo PI' },
      { path: '/measurement/assignments', label: 'Phân công giảng viên' },
      { path: '/measurement/sync-grades', label: 'Nhập – Đồng bộ điểm' },
      { path: '/measurement/rubric-scoring', label: 'Chấm theo Rubric' },
      { path: '/measurement/data-validation', label: 'Kiểm tra dữ liệu' },
      { path: '/measurement/calculation', label: 'Tính toán kết quả' },
      { path: '/measurement/evidence', label: 'Minh chứng đo lường' },
    ],
  },
  {
    id: 'results-cqi',
    label: 'Kết quả và cải tiến',
    icon: TrendingUp,
    badge: 'AUN-QA',
    children: [
      { path: '/results/clo', label: 'Kết quả CLO' },
      { path: '/results/pi', label: 'Kết quả PI' },
      { path: '/results/plo', label: 'Kết quả PLO' },
      { path: '/results/warnings', label: 'Cảnh báo chưa đạt' },
      { path: '/results/summary-reports', label: 'Báo cáo tổng hợp' },
      { path: '/cqi/action-plans', label: 'Kế hoạch cải tiến' },
      { path: '/cqi/monitoring', label: 'Theo dõi cải tiến' },
    ],
  },
  {
    id: 'ai-assistant',
    label: 'Trợ lý dữ liệu',
    icon: Sparkles,
    badge: 'AI RAG',
    children: [
      { path: '/ai/chatbot', label: 'Chatbot truy vấn' },
      { path: '/ai/analytics', label: 'Phân tích dữ liệu' },
      { path: '/ai/early-warnings', label: 'Cảnh báo sớm' },
    ],
  },
  {
    id: 'system-governance',
    label: 'Quản trị hệ thống',
    icon: ShieldCheck,
    children: [
      { path: '/governance/users', label: 'Người dùng' },
      { path: '/governance/roles-scopes', label: 'Vai trò và phân quyền' },
      { path: '/governance/sis-lms-integration', label: 'Tích hợp SIS/LMS' },
      { path: '/governance/audit-logs', label: 'Nhật ký hệ thống' },
      { path: '/governance/system-config', label: 'Cấu hình hệ thống' },
    ],
  },
];

export const Sidebar: React.FC = () => {
  const location = useLocation();
  const [openGroups, setOpenGroups] = useState<Record<string, boolean>>({
    'academic-data': true,
    'curriculum-outcomes': true,
    'syllabus-assessment': true,
    'measurement-scoring': true,
    'results-cqi': true,
    'ai-assistant': true,
    'system-governance': true,
  });
  const [filterQuery, setFilterQuery] = useState('');

  const toggleGroup = (groupId: string) => {
    setOpenGroups((prev) => ({ ...prev, [groupId]: !prev[groupId] }));
  };

  const isGroupActive = (group: NavGroup): boolean => {
    if (group.path && location.pathname === group.path) return true;
    if (group.children) {
      return group.children.some((child) => location.pathname === child.path);
    }
    return false;
  };

  return (
    <aside
      style={{
        position: 'fixed',
        left: 0,
        top: 0,
        bottom: 0,
        width: 'var(--sidebar-width)',
        backgroundColor: 'var(--bg-surface)',
        borderRight: '1px solid var(--border-medium)',
        display: 'flex',
        flexDirection: 'column',
        zIndex: 40,
        padding: '1.25rem 0.75rem',
      }}
    >
      {/* Brand Header */}
      <div
        style={{
          display: 'flex',
          alignItems: 'center',
          gap: '0.75rem',
          padding: '0 0.5rem 1.25rem 0.5rem',
          borderBottom: '1px solid var(--border-subtle)',
        }}
      >
        <div
          style={{
            width: '38px',
            height: '38px',
            borderRadius: 'var(--radius-md)',
            background: 'var(--primary-gradient)',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            color: '#fff',
            boxShadow: '0 4px 14px rgba(99, 102, 241, 0.4)',
            flexShrink: 0,
          }}
        >
          <Cpu size={22} />
        </div>
        <div>
          <h1 style={{ fontSize: '1.05rem', fontWeight: 800, letterSpacing: '-0.02em', color: 'var(--text-primary)' }}>
            DNU OutcomeHub
          </h1>
          <p style={{ fontSize: '0.7rem', color: 'var(--primary-400)', fontWeight: 600 }}>
            Hệ Thống Đo Lường CĐR (OBE)
          </p>
        </div>
      </div>

      {/* Quick Search in Menu */}
      <div style={{ padding: '0.75rem 0.25rem', position: 'relative' }}>
        <Search size={14} style={{ position: 'absolute', left: '0.75rem', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-muted)' }} />
        <input
          type="text"
          value={filterQuery}
          onChange={(e) => setFilterQuery(e.target.value)}
          placeholder="Lọc menu chức năng..."
          style={{
            width: '100%',
            padding: '0.35rem 0.5rem 0.35rem 1.8rem',
            borderRadius: 'var(--radius-sm)',
            border: '1px solid var(--border-subtle)',
            backgroundColor: 'var(--bg-surface-elevated)',
            color: 'var(--text-primary)',
            fontSize: '0.75rem',
          }}
        />
      </div>

      {/* Hierarchical Nav Tree */}
      <nav
        style={{
          marginTop: '0.25rem',
          display: 'flex',
          flexDirection: 'column',
          gap: '0.25rem',
          flex: 1,
          overflowY: 'auto',
          paddingRight: '0.25rem',
        }}
      >
        {navTree.map((group) => {
          const groupActive = isGroupActive(group);
          const hasChildren = group.children && group.children.length > 0;
          const isOpen = openGroups[group.id] || filterQuery.trim().length > 0;

          // Filter children if search query exists
          const filteredChildren = group.children?.filter((child) =>
            child.label.toLowerCase().includes(filterQuery.toLowerCase())
          );

          if (filterQuery.trim().length > 0 && !group.label.toLowerCase().includes(filterQuery.toLowerCase()) && (!filteredChildren || filteredChildren.length === 0)) {
            return null;
          }

          // Single Direct Item (e.g. Tổng quan)
          if (!hasChildren && group.path) {
            return (
              <NavLink
                key={group.id}
                to={group.path}
                style={({ isActive }) => ({
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'space-between',
                  padding: '0.6rem 0.75rem',
                  borderRadius: 'var(--radius-md)',
                  color: isActive ? 'var(--primary-400)' : 'var(--text-primary)',
                  backgroundColor: isActive ? 'rgba(99, 102, 241, 0.15)' : 'transparent',
                  border: isActive ? '1px solid rgba(99, 102, 241, 0.3)' : '1px solid transparent',
                  textDecoration: 'none',
                  fontSize: '0.8125rem',
                  fontWeight: isActive ? 700 : 600,
                  transition: 'all 0.15s ease',
                })}
              >
                <div style={{ display: 'flex', alignItems: 'center', gap: '0.625rem' }}>
                  <group.icon size={17} />
                  <span>{group.label}</span>
                </div>
              </NavLink>
            );
          }

          // Expandable Group
          return (
            <div key={group.id} style={{ marginBottom: '0.25rem' }}>
              <button
                onClick={() => toggleGroup(group.id)}
                style={{
                  width: '100%',
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'space-between',
                  padding: '0.55rem 0.75rem',
                  borderRadius: 'var(--radius-md)',
                  color: groupActive ? 'var(--primary-400)' : 'var(--text-primary)',
                  backgroundColor: groupActive ? 'rgba(99, 102, 241, 0.08)' : 'transparent',
                  border: 'none',
                  fontSize: '0.8125rem',
                  fontWeight: 700,
                  cursor: 'pointer',
                  textAlign: 'left',
                  transition: 'all 0.15s ease',
                }}
              >
                <div style={{ display: 'flex', alignItems: 'center', gap: '0.625rem' }}>
                  <group.icon size={17} />
                  <span>{group.label}</span>
                </div>

                <div style={{ display: 'flex', alignItems: 'center', gap: '0.35rem' }}>
                  {group.badge && (
                    <span
                      style={{
                        fontSize: '0.6rem',
                        fontWeight: 700,
                        padding: '0.1rem 0.35rem',
                        borderRadius: 'var(--radius-full)',
                        backgroundColor: 'rgba(99, 102, 241, 0.15)',
                        color: 'var(--primary-400)',
                      }}
                    >
                      {group.badge}
                    </span>
                  )}
                  {isOpen ? <ChevronDown size={14} /> : <ChevronRight size={14} />}
                </div>
              </button>

              {/* Submenu Accordion Items */}
              {isOpen && (
                <div
                  style={{
                    paddingLeft: '1.5rem',
                    marginTop: '0.15rem',
                    display: 'flex',
                    flexDirection: 'column',
                    gap: '0.15rem',
                    borderLeft: '1px dashed var(--border-medium)',
                    marginLeft: '1.25rem',
                  }}
                >
                  {(filteredChildren || group.children || []).map((sub) => (
                    <NavLink
                      key={sub.path}
                      to={sub.path}
                      style={({ isActive }) => ({
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'space-between',
                        padding: '0.45rem 0.6rem',
                        borderRadius: 'var(--radius-sm)',
                        color: isActive ? 'var(--primary-400)' : 'var(--text-secondary)',
                        backgroundColor: isActive ? 'rgba(99, 102, 241, 0.15)' : 'transparent',
                        fontWeight: isActive ? 700 : 500,
                        fontSize: '0.75rem',
                        textDecoration: 'none',
                        transition: 'all 0.15s ease',
                      })}
                    >
                      <span>• {sub.label}</span>
                      {sub.badge && (
                        <span style={{ fontSize: '0.6rem', padding: '0.1rem 0.3rem', borderRadius: '4px', backgroundColor: 'rgba(99, 102, 241, 0.2)', color: 'var(--primary-400)' }}>
                          {sub.badge}
                        </span>
                      )}
                    </NavLink>
                  ))}
                </div>
              )}
            </div>
          );
        })}
      </nav>

      {/* Footer System Health */}
      <div
        style={{
          marginTop: 'auto',
          padding: '0.75rem 1rem',
          backgroundColor: 'var(--bg-surface-elevated)',
          borderRadius: 'var(--radius-md)',
          border: '1px solid var(--border-subtle)',
        }}
      >
        <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', marginBottom: '0.2rem' }}>
          <span style={{ width: '8px', height: '8px', borderRadius: '50%', backgroundColor: 'var(--emerald-400)', boxShadow: '0 0 8px var(--emerald-400)' }} />
          <span style={{ fontSize: '0.72rem', fontWeight: 700, color: 'var(--text-primary)' }}>
            100% Phân Hệ Hoạt Động
          </span>
        </div>
        <p style={{ fontSize: '0.68rem', color: 'var(--text-muted)' }}>
          8 Nhóm / 40+ Chức năng chuẩn OBE
        </p>
      </div>
    </aside>
  );
};
