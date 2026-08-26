import React, { useState } from 'react';
import {
  ShieldCheck,
  Lock,
  UserCheck,
  History,
  Key,
  CheckCircle,
  AlertTriangle,
  FileCode,
} from 'lucide-react';

export const GovernanceIamPage: React.FC = () => {
  const [activeTab, setActiveTab] = useState<'roles' | 'sod' | 'audit'>('audit');

  return (
    <div className="animate-fade-in">
      {/* Header */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <div>
          <h2 style={{ fontSize: '1.75rem', fontWeight: 800, color: 'var(--text-primary)', letterSpacing: '-0.02em' }}>
            Quản Trị Người Dùng, Phân Quyền & Audit Trail (Mục 8.9)
          </h2>
          <p style={{ color: 'var(--text-secondary)', fontSize: '0.875rem', marginTop: '0.25rem' }}>
            Phân quyền đa tầng theo Scope, kiểm soát Tách biệt nhiệm vụ (Separation of Duties - SoD) và chuỗi Audit Log bất biến.
          </p>
        </div>

        <button className="btn btn-primary">
          <Key size={16} />
          <span>Gán Vai Trò Theo Scope</span>
        </button>
      </div>

      {/* Tabs */}
      <div style={{ display: 'flex', gap: '0.5rem', marginBottom: '1.5rem', borderBottom: '1px solid var(--border-medium)', paddingBottom: '0.5rem' }}>
        {[
          { key: 'audit', label: 'Chuỗi Băm Audit Log Bất Biến (Hash Chain)', icon: History, badge: 'SHA-256' },
          { key: 'sod', label: 'Tách Biệt Nhiệm Vụ (Separation of Duties)', icon: Lock },
          { key: 'roles', label: 'Vai Trò & Quyền Hạn Theo Scope', icon: UserCheck },
        ].map((tab) => (
          <button
            key={tab.key}
            onClick={() => setActiveTab(tab.key as any)}
            className={`btn ${activeTab === tab.key ? 'btn-primary' : 'btn-secondary'}`}
            style={{ fontSize: '0.8125rem' }}
          >
            <tab.icon size={16} />
            <span>{tab.label}</span>
            {tab.badge && <span className="badge badge-bloom badge-cyan">{tab.badge}</span>}
          </button>
        ))}
      </div>

      {/* TAB 1: AUDIT TRAIL IMMUTABLE HASH CHAIN (FR-ADM-05) */}
      {activeTab === 'audit' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">
                <History size={20} className="text-emerald-400" />
                Chuỗi Nhật Ký Kiểm Toán Bất Biến (Immutable Hash Chain - FR-ADM-05)
              </h3>
              <p className="glass-card-subtitle">
                Mỗi hành động thay đổi cấu hình, nhập/xuất điểm hoặc phê duyệt đều được gắn mã băm liên kết không thể sửa xóa
              </p>
            </div>
            <span className="badge badge-success">Toàn Vẹn 100% (Không Vi Phạm)</span>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Thời Điểm (UTC)</th>
                  <th>Người Thực Hiện</th>
                  <th>Hành Động Nghiệp Vụ</th>
                  <th>Đối Tượng Tác Động</th>
                  <th>Scope Dữ Liệu</th>
                  <th>Mã Băm Bất Biến (Current Hash)</th>
                </tr>
              </thead>
              <tbody>
                {[
                  { time: '26/08/2026 13:30:15', actor: 'TS. Lê Hải Nam (GV001)', action: 'CHẤM ĐIỂM RUBRIC A2', target: 'Lớp 17IT01 - Môn Lập trình .NET', scope: 'Khoa CNTT', hash: 'e4f5a6b7...c8d9' },
                  { time: '26/08/2026 11:15:40', actor: 'Admin Hệ Thống', action: 'XOAY VÒNG KHÓA API KEY', target: 'Service Account sa_sis_sync', scope: 'Toàn Trường', hash: '7a8b9c0d...1e2f' },
                  { time: '25/08/2026 16:45:22', actor: 'PGS. TS. Trần Văn Bình', action: 'PHÊ DUYỆT ĐỀ CƯƠNG BM13', target: 'Học phần IT4101 v2.0', scope: 'Ngành KTPM', hash: '3d4e5f6a...7b8c' },
                  { time: '25/08/2026 09:10:05', actor: 'ThS. Nguyễn Văn Toàn', action: 'KHỞI TẠO KẾ HOẠCH CQI', target: 'Kế hoạch CQI-2024-IT-02', scope: 'Khoa CNTT', hash: '1a2b3c4d...5e6f' },
                ].map((log, i) => (
                  <tr key={i}>
                    <td><code style={{ fontSize: '0.75rem' }}>{log.time}</code></td>
                    <td><strong>{log.actor}</strong></td>
                    <td><span className="badge badge-primary">{log.action}</span></td>
                    <td>{log.target}</td>
                    <td><span className="badge badge-secondary">{log.scope}</span></td>
                    <td><code style={{ fontSize: '0.75rem', color: 'var(--emerald-400)' }}>{log.hash}</code></td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 2: SEPARATION OF DUTIES (FR-ADM-04) */}
      {activeTab === 'sod' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Chính Sách Tách Biệt Nhiệm Vụ (Separation of Duties - SoD)</h3>
              <p className="glass-card-subtitle">Ngăn chặn xung đột lợi ích: Người nhập điểm không được tự phê duyệt/công bố kết quả</p>
            </div>
            <span className="badge badge-success">3 Quy Tắc SoD Kích Hoạt</span>
          </div>

          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(320px, 1fr))', gap: '1rem' }}>
            {[
              { rule: 'SoD-01: Nhập Điểm vs Phê Duyệt', desc: 'Giảng viên chấm thi không được kiêm nhiệm quyền Phê duyệt kết quả đợt đo', status: 'TUÂN THỦ' },
              { rule: 'SoD-02: Soạn Thảo vs Thẩm Định ĐCCT', desc: 'Tác giả biên soạn Đề cương chi tiết không được tự ký duyệt thẩm định', status: 'TUÂN THỦ' },
              { rule: 'SoD-03: Quản Trị Hệ Thống vs Nhập Điểm', desc: 'Tài khoản Quản trị viên (Admin) không được can thiệp sửa điểm trực tiếp', status: 'TUÂN THỦ' },
            ].map((r, idx) => (
              <div key={idx} style={{ padding: '1.25rem', backgroundColor: 'var(--bg-surface-elevated)', borderRadius: 'var(--radius-md)', border: '1px solid var(--border-medium)' }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '0.5rem' }}>
                  <strong style={{ color: 'var(--primary-400)' }}>{r.rule}</strong>
                  <span className="badge badge-success">{r.status}</span>
                </div>
                <p style={{ fontSize: '0.8rem', color: 'var(--text-secondary)' }}>{r.desc}</p>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* TAB 3: ROLES & SCOPES (FR-ADM-02, 03) */}
      {activeTab === 'roles' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Phân Quyền Ma Trận Vai Trò Theo Scope (Khoa - CTĐT - Lớp - Đợt)</h3>
              <p className="glass-card-subtitle">Server-side RLS enforcement đảm bảo không rò rỉ dữ liệu ngoài thẩm quyền</p>
            </div>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Tên Vai Trò</th>
                  <th>Phạm Vi Scope</th>
                  <th>Quyền Đọc (SELECT)</th>
                  <th>Quyền Ghi (INSERT/UPDATE)</th>
                  <th>Quyền Phê Duyệt</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td><strong>ADMIN (Quản trị hệ thống)</strong></td>
                  <td><span className="badge badge-primary">Toàn Trường</span></td>
                  <td>Toàn bộ hệ thống</td>
                  <td>Cấu hình, Người dùng, System Params</td>
                  <td>Phân quyền & Khóa hệ thống</td>
                </tr>
                <tr>
                  <td><strong>DEAN (Trưởng Khoa / Viện)</strong></td>
                  <td><span className="badge badge-cyan">Khoa CNTT</span></td>
                  <td>Dữ liệu trong Khoa</td>
                  <td>Phê duyệt CTĐT, Đề cương, CQI</td>
                  <td>Công bố kết quả CĐR Khoa</td>
                </tr>
                <tr>
                  <td><strong>LECTURER (Giảng viên)</strong></td>
                  <td><span className="badge badge-secondary">Lớp Được Phân Công</span></td>
                  <td>Lớp giảng dạy & Môn phụ trách</td>
                  <td>Nhập điểm Rubric, Soạn ĐCCT</td>
                  <td>Ký xác nhận điểm lớp</td>
                </tr>
                <tr>
                  <td><strong>STUDENT (Sinh viên)</strong></td>
                  <td><span className="badge badge-secondary">Cá Nhân</span></td>
                  <td>Điểm CĐR cá nhân & CTĐT đang học</td>
                  <td>Không có quyền ghi</td>
                  <td>Không có</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
};
