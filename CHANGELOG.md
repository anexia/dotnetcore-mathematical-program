# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [2.6.3] - 2025-11-06

### Added
- Release MPaaS Client

## [2.4.0] - 2025-10-01

### Added

- Added possibility to use CBC Solver as Fallback ILP Solver.

## [2.2.0] - 2025-03-26

### Added

- Added possibility to pass an MPS model as string to the LP solver.
- Added possibility to pass additional solver specific parameters to the solvers.
- Added public members in ICompletedOptimizationModel.
- Added class IntegralPoint for integral intervals.

### Fixed

- Fixed bug where the GLOP solver could not be used with NumberOfThreads parameter.

## [2.1.0] - 2025-03-05

### Added

- Added support for HiGHS solver for ILP Problems.
- Added possibility to pass an MPS model as string to the ILP solver.

## [2.0.1] - 2025-01-26

- Internal optimizations.

## [2.0.0] - 2025-01-22

### Added

- Introduce a completely new way of creating solvers and model.
- Added support for SCIP solver.
- Added support for solving CP models.

### Deprecated

- Usage of CBC solver has been marked as deprecated.

### Removed

- Removed various classes as there is a new way of creating models als solvers.

## [1.0.3] - 2025-01-22

### Fixed

- Fixed bug where two terms with the same variable could not be added to the same terms list.

## [1.0.2] - 2024-12-05

### Added

- Initial release with basic options for ILP and LP solving.