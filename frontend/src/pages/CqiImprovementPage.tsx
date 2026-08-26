import React, { useState } from 'react';
import { useLocation } from 'react-router-dom';
import {
  RefreshCw,
  Plus,
  Clock,
  CheckCircle2,
  AlertTriangle,
  ArrowRight,
  ShieldCheck,
  FileCheck,
  TrendingUp,
} from 'lucide-react';

interface CqiPlanItem {
  id: string;
  code: string;
  title: string;
  ploTarget: string;
  rootCause: string;
  assignedStaff: string;
  deadline: string;
  status: 'DRAFT' | 'APPROVED' | 'IN_PROGRESS' | 'VERIFIED_CLOSED';
  progressPercentage: number;
}

export const CqiImprovementPage: React.FC = () => {
  const location = useLocation();
  const isMonitoringView = location.pathname.includes('/cqi/monitoring');
  const [activeTab, setActiveTab] = useState<'action-plans' | 'monitoring'>(isMonitoringView ? 'monitoring' : 'action-plans');

  const [plans, setPlans] = useState<CqiPlanItem[]>([
    {
      id: 'cqi-01',
      code: 'CQI-2023-IT-01',
      title: 'Tăng cường thời lượng thực hành Unit Test & CI/CD môn Lập trình .NET',
      ploTarget: 'PLO5 (Kiểm thử)',
      rootCause: 'Sinh viên ít được tiếp cận công cụ kiểm thử tự động trong các học phần cơ sở',
      assignedStaff: 'TS. Lê Hải Nam',
      deadline: '30/11/2023',
      status: 'VERIFIED_CLOSED',
      progressPercentage: 100,
    },
    {
      id: 'cqi-02',
      code: 'CQI-2024-IT-02',
      title: 'Bổ sung chuyên đề Thiết kế Cơ sở Dữ liệu NoSQL & MongoDB',
      ploTarget: 'PLO2 (Thiết kế hệ thống)',
      rootCause: 'Đề cương cũ tập trung chủ yếu vào RDBMS SQL truyền thống',
      assignedStaff: 'ThS. Nguyễn Văn Toàn',
      deadline: '15/05/2024',
      status: 'IN_PROGRESS',
      progressPercentage: 65,
    },
    {
      id: 'cqi-03',
      code: 'CQI-2024-IT-03',
      title: 'Tổ chức Workshop Kỹ năng Giao tiếp & Thuyết trình Kỹ thuật',
      ploTarget: 'PLO4 (Kỹ năng mềm)',
      rootCause: 'Sinh viên thiếu tự tin khi trình bày trước hội đồng đồ án',
      assignedStaff: 'TS. Trần Mai Hương',
      deadline: '20/06/2024',
      status: 'APPROVED',
      progressPercentage: 20,
    },
  ]);

  return (
    <div className="animate-fade-in">
      {/* Header */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <div>
          <h2 style={{ fontSize: '1.75rem', fontWeight: 800, color: 'var(--text-primary)', letterSpacing: '-0.02em' }}>
            Cải Tiến Chất Lượng Liên Tục (CQI Management - Mục 8.7)
          </h2>
          <p style={{ color: 'var(--text-secondary)', fontSize: '0.875rem', marginTop: '0.25rem' }}>
            Quản lý vòng lặp khép kín: Phát hiện vấn đề ➔ Phân tích nguyên nhân 5-Why ➔ Kế hoạch hành động ➔ Đo lường lại ➔ Nghiệm thu đóng.
          </p>
        </div>

        <button className="btn btn-primary">
          <Plus size={16} />
          <span>Tạo Kế Hoạch CQI Mới</span>
        </button>
      </div>

      {/* Sub Tabs */}
      <div style={{ display: 'flex', gap: '0.5rem', marginBottom: '1.5rem', borderBottom: '1px solid var(--border-medium)', paddingBottom: '0.5rem' }}>
        <button
          onClick={() => setActiveTab('action-plans')}
          className={`btn ${activeTab === 'action-plans' ? 'btn-primary' : 'btn-secondary'}`}
          style={{ fontSize: '0.8125rem' }}
        >
          <RefreshCw size={16} />
          <span>Bảng Kế Hoạch Cải Tiến (Kanban)</span>
        </button>
        <button
          onClick={() => setActiveTab('monitoring')}
          className={`btn ${activeTab === 'monitoring' ? 'btn-primary' : 'btn-secondary'}`}
          style={{ fontSize: '0.8125rem' }}
        >
          <TrendingUp size={16} />
          <span>Theo Dõi Tiến Độ & Đo Lường Lại</span>
        </button>
      </div>

      {/* TAB 1: KANBAN BOARD */}
      {activeTab === 'action-plans' && (
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(320px, 1fr))', gap: '1.25rem' }}>
          {/* Column 1: Đã Phê Duyệt */}
          <div className="glass-card" style={{ padding: '1.25rem' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem', borderBottom: '1px solid var(--border-subtle)', paddingBottom: '0.5rem' }}>
              <span style={{ fontWeight: 700, fontSize: '0.9rem', color: 'var(--primary-400)' }}>1. Đã Phê Duyệt (Chờ triển khai)</span>
              <span className="badge badge-primary">1 Kế hoạch</span>
            </div>
            {plans.filter((p) => p.status === 'APPROVED').map((plan) => (
              <div key={plan.id} style={{ backgroundColor: 'var(--bg-surface-elevated)', padding: '1rem', borderRadius: 'var(--radius-md)', border: '1px solid var(--border-medium)', marginBottom: '0.75rem' }}>
                <span className="badge badge-secondary" style={{ marginBottom: '0.5rem', display: 'inline-block' }}>{plan.code}</span>
                <h4 style={{ fontSize: '0.9rem', fontWeight: 700, color: 'var(--text-primary)', marginBottom: '0.35rem' }}>{plan.title}</h4>
                <p style={{ fontSize: '0.75rem', color: 'var(--text-secondary)', marginBottom: '0.5rem' }}>• Mục tiêu: <strong>{plan.ploTarget}</strong></p>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', fontSize: '0.7rem', color: 'var(--text-muted)' }}>
                  <span>Phụ trách: {plan.assignedStaff}</span>
                  <span>Hạn: {plan.deadline}</span>
                </div>
              </div>
            ))}
          </div>

          {/* Column 2: Đang Thực Hiện */}
          <div className="glass-card" style={{ padding: '1.25rem' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem', borderBottom: '1px solid var(--border-subtle)', paddingBottom: '0.5rem' }}>
              <span style={{ fontWeight: 700, fontSize: '0.9rem', color: 'var(--cyan-400)' }}>2. Đang Thực Hiện</span>
              <span className="badge badge-cyan">1 Kế hoạch</span>
            </div>
            {plans.filter((p) => p.status === 'IN_PROGRESS').map((plan) => (
              <div key={plan.id} style={{ backgroundColor: 'var(--bg-surface-elevated)', padding: '1rem', borderRadius: 'var(--radius-md)', border: '1px solid var(--cyan-500)', marginBottom: '0.75rem' }}>
                <span className="badge badge-cyan" style={{ marginBottom: '0.5rem', display: 'inline-block' }}>{plan.code}</span>
                <h4 style={{ fontSize: '0.9rem', fontWeight: 700, color: 'var(--text-primary)', marginBottom: '0.35rem' }}>{plan.title}</h4>
                <p style={{ fontSize: '0.75rem', color: 'var(--text-secondary)', marginBottom: '0.5rem' }}>• Nguyên nhân: {plan.rootCause}</p>
                <div style={{ width: '100%', height: '6px', backgroundColor: 'var(--bg-surface-hover)', borderRadius: 'var(--radius-full)', overflow: 'hidden', margin: '0.5rem 0' }}>
                  <div style={{ width: `${plan.progressPercentage}%`, height: '100%', background: 'var(--cyan-gradient)' }} />
                </div>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', fontSize: '0.7rem', color: 'var(--text-muted)' }}>
                  <span>Tiến độ: {plan.progressPercentage}%</span>
                  <span>Hạn: {plan.deadline}</span>
                </div>
              </div>
            ))}
          </div>

          {/* Column 3: Đã Đo Lại & Đóng */}
          <div className="glass-card" style={{ padding: '1.25rem' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem', borderBottom: '1px solid var(--border-subtle)', paddingBottom: '0.5rem' }}>
              <span style={{ fontWeight: 700, fontSize: '0.9rem', color: 'var(--emerald-400)' }}>3. Đã Đo Lại & Đóng Kế Hoạch</span>
              <span className="badge badge-success">1 Hoàn thành</span>
            </div>
            {plans.filter((p) => p.status === 'VERIFIED_CLOSED').map((plan) => (
              <div key={plan.id} style={{ backgroundColor: 'var(--bg-surface-elevated)', padding: '1rem', borderRadius: 'var(--radius-md)', border: '1px solid var(--emerald-500)', marginBottom: '0.75rem' }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '0.5rem' }}>
                  <span className="badge badge-success">{plan.code}</span>
                  <span style={{ fontSize: '0.7rem', color: 'var(--emerald-400)', fontWeight: 700, display: 'flex', alignItems: 'center', gap: '0.25rem' }}>
                    <ShieldCheck size={12} /> Đã nghiệm thu
                  </span>
                </div>
                <h4 style={{ fontSize: '0.9rem', fontWeight: 700, color: 'var(--text-primary)', marginBottom: '0.35rem' }}>{plan.title}</h4>
                <p style={{ fontSize: '0.75rem', color: 'var(--emerald-400)', marginBottom: '0.5rem' }}>
                  • Kết quả đo lại: Tỷ lệ đạt PLO5 tăng từ <strong>68.0% ➔ 74.5%</strong> (+6.5%)
                </p>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', fontSize: '0.7rem', color: 'var(--text-muted)' }}>
                  <span>Nghiệm thu: TS. Lê Hải Nam</span>
                  <button className="btn btn-sm btn-secondary">Xem Minh Chứng</button>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* TAB 2: MONITORING VIEW */}
      {activeTab === 'monitoring' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Bảng Theo Dõi Đối Sánh Kết Quả Trước & Sau Cải Tiến CQI</h3>
              <p className="glass-card-subtitle">Minh chứng phục vụ các đoàn đánh giá ngoài kiểm định chất lượng AUN-QA / ABET</p>
            </div>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Mã Kế Hoạch CQI</th>
                  <th>Học Phần & CĐR Tác Động</th>
                  <th>Hành Động Đã Triển Khai</th>
                  <th>Tỷ Lệ Đạt Trước CQI</th>
                  <th>Tỷ Lệ Đạt Sau Khi Đo Lại</th>
                  <th>Mức Độ Cải Thiện</th>
                  <th>Trạng Thái Đóng</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td><code>CQI-2023-IT-01</code></td>
                  <td>IT4101 .NET (<strong>PLO5 Kiểm thử</strong>)</td>
                  <td>Bổ sung 4 bài lab Unit Test & CI/CD</td>
                  <td>68.0%</td>
                  <td><strong style={{ color: 'var(--emerald-400)' }}>74.5%</strong></td>
                  <td><span className="badge badge-success">+6.5% ↑</span></td>
                  <td><span className="badge badge-success">ĐÃ NGHIỆM THU ĐÓNG</span></td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
};
