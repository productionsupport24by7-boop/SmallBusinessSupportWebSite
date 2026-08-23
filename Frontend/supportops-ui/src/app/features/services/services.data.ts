import { Service } from '../../core/models/service.model';

export const SERVICES: Service[] = [
  {
    id: 1,
    title: 'Production Support',
    icon: 'support_agent',
    description: '24×7 application support to keep your business running.',
    features: [
      'Incident Management',
      'Root Cause Analysis',
      'Application Monitoring',
      'SLA Management',
      'MTTR and MTTA Analysis'
    ]
  },
  {
    id: 2,
    title: 'SQL Server',
    icon: 'storage',
    description: 'Database performance tuning and troubleshooting.',
    features: [
      'Performance Tuning',
      'Deadlock Analysis',
      'Index Optimization',
      'Backup & Recovery'
    ]
  },
  {
    id: 3,
    title: 'Cloud & Azure',
    icon: 'cloud',
    description: 'Support for Microsoft Azure infrastructure and applications.',
    features: [
      'Azure App Service',
      'Azure SQL',
      'Application Insights',
      'Azure Monitor'
    ]
  },
  {
    id: 4,
    title: 'DevOps',
    icon: 'settings',
    description: 'Automated deployments and release management.',
    features: [
      'CI/CD',
      'Azure DevOps',
      'Deployment Automation',
      'Release Support'
    ]
  }
];
